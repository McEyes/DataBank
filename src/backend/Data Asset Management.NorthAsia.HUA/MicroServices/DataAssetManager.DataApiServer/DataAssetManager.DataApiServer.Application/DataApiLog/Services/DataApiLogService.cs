using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch.Security;

using Furion.EventBus;

using Grpc.Core;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;
using ITPortal.Core.WebApi;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System.Text;

using static Grpc.Core.Metadata;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataApiLogService : BaseService<DataApiLogEntity, DataApiLogDto, string>, IDataApiLogService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public DataApiLogService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, false)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<DataApiLogEntity> BuildFilterQuery(DataApiLogDto filter)
        {
            return CurrentDb.Queryable<DataApiLogEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(filter.Time > 0, f => f.Time >= filter.Time)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ApiId), f => f.ApiId == filter.ApiId)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ApiName), f => SqlFunc.ToLower(f.ApiName).Contains(filter.ApiName.ToLower()) || SqlFunc.ToLower(f.ApiName).Contains(filter.ApiName.ToLower()) || f.CallerIp.Equals(filter.ApiName) || f.Msg.Contains(filter.ApiName.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.CallerUrl), f => SqlFunc.ToLower(f.CallerUrl).Contains(filter.CallerUrl.ToLower()))
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .OrderByDescending(f => f.CallerDate);
        }


        public async Task<List<DayVisitedDto>> GetApiDailyStats(DateTime startDate)
        {
            return await CurrentDb.Queryable<DayVisitedDto>().Where(f => f.Daily >= startDate).ToListAsync();
            //return await CurrentDb.Queryable<DataApiLogEntity>().Where(f => f.CallerDate >= startDate).GroupBy(f => SqlFunc.ToDateShort(f.CallerDate))
            //.Select(f => new DayVisitedDto { Daily = SqlFunc.ToDateShort(f.CallerDate), Visited = SqlFunc.AggregateDistinctCount(f.Id) })
            //.ToListAsync();
        }

        public async Task SendLogEvent(HttpContext context, RouteInfo routInfo, int times, string msg, int status = 0, long callerSize = 0)
        {
            //记录api调用情况
            Dictionary<string, object> paramsData = await GetAllParams(context);
            var userid = context.GetCurrUserInfo()?.UserId;
            if (userid.IsNullOrWhiteSpace())
            {
                userid = App.HttpContext.Request.Headers["x-token"];
            }
            var apiLogDto = new DataApiLogEntity()
            {
                //Id = Guid.NewGuid().ToString().Replace("-", ""), 
                ApiId = routInfo.Id,
                ApiName = routInfo.ApiName,
                CallerUrl = routInfo.ApiServiceUrl,
                CallerParams = paramsData.ToJSON(),// JSON.Serialize(paramsData), // ToKeyValueStr(paramsData),
                CallerId = userid,
                CallerDate = DateTimeOffset.Now,
                CallerIp = IPAddressUtils.GetClientIp(context.Request),
                CallerSize = callerSize,
                OwnerDepart = routInfo.OwnerDepart,
                Status = status,
                Time = times,
                Msg = msg,
                TableId = routInfo.TableId,
            };
            if (context.Items.TryGetValue("ApiTrackLog", out object logInfo))
            {
                var trackingInfo = logInfo as ApiTrackLogInfo;
                apiLogDto.CallerDate = trackingInfo.CallerDate;
                apiLogDto.Time = (int)(DateTimeOffset.Now - apiLogDto.CallerDate).TotalMilliseconds;
            }
            var _ = _eventPublisher.PublishAsync(DataAssetManagerConst.LogRecordEvent, apiLogDto);
        }

        public async Task<Dictionary<string, object>> GetAllParams(HttpContext context)
        {
            var request = context.Request;
            var paramsData = await ReadRequestBody(context);
            //ToDynamic(headers: request.Headers, paramsData);
            paramsData = ToDynamic(request.Query, paramsData);
            if (request.HasFormContentType) paramsData = ToDynamic(request.Form, paramsData);
            CheckPageParams(paramsData);
            return paramsData;
        }

        private async Task<Dictionary<string, object>> ReadRequestBody(HttpContext context)
        {
            return await ReadRequestBody<Dictionary<string, object>>(context);
        }

        private async Task<T> ReadRequestBody<T>(HttpContext context)
        {
            // 确保请求体流可重复读取
            context.Request.EnableBuffering();
            if (!context.Request.ContentLength.HasValue || context.Request.ContentLength <= 0) return default;
            // 读取请求体
            //var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            //await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            //var requestBody = Encoding.UTF8.GetString(buffer);
            var requestBody = string.Empty;
            // 初始化内存流用于累积数据
            using (var ms = new MemoryStream())
            {
                // 定义每次读取的缓冲区（大小可根据实际场景调整，例如4096字节）
                byte[] buffer = new byte[4096];
                int bytesRead;

                // 循环读取，直到流结束（bytesRead为0）
                while ((bytesRead = await context.Request.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    // 将读取到的字节写入内存流
                    await ms.WriteAsync(buffer, 0, bytesRead);
                }

                // 将内存流中的数据转换为字符串（使用UTF8编码）
                requestBody = Encoding.UTF8.GetString(ms.ToArray());
            }
            // 输出请求体数据
            //System.Console.WriteLine($"Request Body: {requestBody}");
            requestBody = requestBody.Trim('\0');
            // 将流指针重置到开头，以便后续中间件或控制器可以再次读取请求体
            context.Request.Body.Position = 0;
            return Furion.JsonSerialization.JSON.Deserialize<T>(requestBody);
        }
        private Dictionary<string, object> ToDynamic(IFormCollection from, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (from == null) return expando;

            foreach (var kvp in from)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }
        private Dictionary<string, object> ToDynamic(IQueryCollection query, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (query == null) return expando;

            foreach (var kvp in query)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        private void CheckPageParams(Dictionary<string, object> expandoDict)
        {
            if (!expandoDict.ContainsKey("pageNum")) expandoDict.Add("pageNum", 1);
            if (!expandoDict.ContainsKey("pageSize"))
            {
                if (!expandoDict.ContainsKey("maxSize"))
                    expandoDict.Add("pageSize", value: 1000);
                else
                    expandoDict.Add("pageSize", expandoDict["maxSize"]);
            }
            if (!expandoDict.ContainsKey("total")) expandoDict.Add("total", 0);
        }



        public async Task<PageResult<DataApiLogView>> LogViewQuery(DataApiLogQuery filter)
        {
            if (filter.StartDate.HasValue)
            {
                filter.StartDate = filter.StartDate.Value.Date;
            }
            if (filter.EndDate.HasValue)
            {
                filter.EndDate = filter.EndDate.Value.Date.AddDays(1);
            }
            var query = CurrentDb.Queryable<DataApiLogView>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(filter.Time > 0, f => f.Time >= filter.Time)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ApiName), f => SqlFunc.ToLower(f.CallerUrl).Contains(filter.ApiName.ToLower()) || SqlFunc.ToLower(f.ApiName).Contains(filter.ApiName.ToLower()) || SqlFunc.ToLower(f.CallerName).Contains(filter.ApiName.ToLower()) || f.CallerIp.Equals(filter.ApiName))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.OwnerName), f => SqlFunc.ToLower(f.OwnerName).Contains(filter.OwnerName.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.OwnerDepart), f => SqlFunc.ToLower(f.OwnerDepart).Contains(filter.OwnerDepart.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.CallerName), f => SqlFunc.ToLower(f.CallerName).Contains(filter.CallerName.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.CallerIp), f => f.CallerIp.Contains(filter.CallerIp))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.CallerUrl), f => SqlFunc.ToLower(f.CallerUrl).Contains(filter.CallerUrl.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Owner), f => SqlFunc.ToLower(f.Owner).Contains(filter.Owner.ToLower()))
                .WhereIF(filter.CallerSize.HasValue, f => f.CallerSize > filter.CallerSize)
                .WhereIF(filter.StartDate.HasValue, f => f.CallerDate > filter.StartDate)
                .WhereIF(filter.EndDate.HasValue, f => f.CallerDate < filter.EndDate)
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .OrderByDescending(f => f.CallerDate);
            return new PageResult<DataApiLogView>(query.Count(), await Page(query, filter).ToListAsync(), filter.PageNum, filter.PageSize);
        }
    }
}
