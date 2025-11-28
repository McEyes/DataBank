using DataAssetManager.DataTableServer.Application;

using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.Emails;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DataAssetManager.DataApiServer.Application.EventSub
{
    public class CacheRefreshEventSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<CacheRefreshEventSubscriber> _logger;
        private readonly IDataTableService _dataTableService;
        private readonly IDataApiService _apiService;
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IAssetClientsService _assetClientService;
        private readonly IEmailSender _emailSender;
        public CacheRefreshEventSubscriber(ILogger<CacheRefreshEventSubscriber> logger
            , IDataTableService dataTableService
            , IDataApiService apiService
            , IDataCatalogService dataCatalogService,
            IEmailSender emailSender,
            IAssetClientsService assetClientService)
        {
            _dataTableService = dataTableService;
            _apiService = apiService;
            _dataCatalogService = dataCatalogService;
            _assetClientService = assetClientService;
            _emailSender = emailSender;
            _logger = logger;
        }

        public bool IsRunRefreshTableRedis = false;

        //public async Task RefreshTableRedis(EventHandlerExecutingContext context)
        //{
        //    try
        //    {
        //        if (!IsRunRefreshTableRedis)
        //        {
        //            IsRunRefreshTableRedis = true;
        //            IsRunRefreshTableRedis = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation(message: $"更新table信息异常：{ex.Message},\r\n{ex.StackTrace}");
        //    }
        //    finally
        //    {
        //        IsRunRefreshTableRedis = false;
        //    }
        //}

        public static Dictionary<string,List<DateTime>> keyValuePairs = new Dictionary<string, List<DateTime>>();
        public bool IsRunRefreshClientRedis = false;
        [EventSubscribe(DataAssetManagerConst.DataAuthRefreshEvent)]
        [EventSubscribe(DataAssetManagerConst.DataTable_UserHashKey)]
        [EventSubscribe(DataAssetManagerConst.DataTable_HashKey)]
        //[EventSubscribe(DataAssetManagerConst.DataApis_HashKey)]
        //[EventSubscribe(DataAssetManagerConst.RouteRedisListKey)]
        public async Task RefreshClientAuthRedis(EventHandlerExecutingContext context)
        {
            _logger.LogInformation(message: $"更新RefreshClientAuthRedis信息：{context.Source.EventId}");
            try
            {
                await Task.Delay(5000);
                if (!IsRunRefreshClientRedis)
                {
                    IsRunRefreshClientRedis = true;
                    var sw = new Stopwatch();
                    sw.Start();
                    var hostUrl = App.GetConfig<string>("RemoteApi:DataAssetHostUrl");
                    if (!hostUrl.IsNullOrWhiteSpace()) DataAssetManagerConst.HostUrl = hostUrl;
                    //await _dataCatalogService.RefreshCache();
                    //await _dataCatalogService.InitRedisHash();
                    //await _assetClientService.RefreshCache();
                    await _apiService.AllFromCache(true);
                    await _apiService.InitRoutes(true);
                    await _dataTableService.InitRedisHash(true);
                    await _dataTableService.InitTableUserFromCache(true);
                    await _assetClientService.InitClientScopes(true);
                    IsRunRefreshClientRedis = false;
                    sw.Stop();
                    _logger.LogInformation(message: $"更新RefreshClientAuthRedis信息：{context.Source.EventId} 更新成功,耗时：{sw.ElapsedMilliseconds}");
                    //await _apiService.InitRedisHash();
                }
                //else if(keyValuePairs.Values.Max(f=>f))
                //{

                //}
                if (!keyValuePairs.ContainsKey(context.Source.EventId))
                    keyValuePairs.TryAdd(context.Source.EventId, new List<DateTime>());
                else keyValuePairs[context.Source.EventId].Add(DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"更新RefreshClientAuthRedis信息异常：{ex.Message},\r\n{ex.StackTrace}");
                await _emailSender.SendAsync(new EmailMessage()
                {
                    MailTo = new string[] { "Yang_Li9954@jabil.com" },
                    Subject = $"刷新缓存异常",
                    Html = @$"Hi:<br>     {ex.Message},\r\n {ex.StackTrace}"
                });
            }
            finally
            {
                IsRunRefreshClientRedis = false;
            }
            if (keyValuePairs.Count % 2 == 0)
            {
                var msg = new StringBuilder();
                foreach (var kv in keyValuePairs)
                    msg.AppendLine($"{kv.Key}:{kv.Value.Count()}");
                keyValuePairs.Clear();
                await _emailSender.SendAsync(new EmailMessage()
                {
                    MailTo = new string[] { "Yang_Li9954@jabil.com" },
                    Subject = $"刷新缓存频率统计邮件通知",
                    Html = @$"Hi:<br>     {msg}"
                });
            }
            _logger.LogError($"Refresh Client Auth Redis:{context.Source.EventId}, {keyValuePairs.Values.Sum(f => f.Count)}");
        }


        public bool IsRunRefreshAPIRedis = false;
        [EventSubscribe(DataAssetManagerConst.DataApis_HashKey)]
        [EventSubscribe(DataAssetManagerConst.RouteRedisListKey)]
        public async Task RefreshAPIRedis(EventHandlerExecutingContext context)
        {
            try
            {
                if (!IsRunRefreshAPIRedis)
                {
                    IsRunRefreshAPIRedis = true;
                    //await Task.Delay(5000);
                    await _apiService.InitRoutes(true);
                    await _apiService.AllFromCache(true);
                    IsRunRefreshAPIRedis = false;
                    //await _apiService.InitRedisHash();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"更新table信息异常：{ex.Message},\r\n{ex.StackTrace}");
            }
            finally
            {
                IsRunRefreshAPIRedis = false;
            }
        }



    }
}
