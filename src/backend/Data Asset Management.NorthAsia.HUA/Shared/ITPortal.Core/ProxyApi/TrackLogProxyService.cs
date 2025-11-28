

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;

using Furion;
using Furion.HttpRemote;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Emails;
using ITPortal.Core.Extensions;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Logging;

using Result = ITPortal.Core.Services.Result;

namespace ITPortal.Core.ProxyApi
{


    /// <summary>
    /// 员工数据获取代理接口
    /// 实现了对/userapi/Employee/info接口的数据获取
    /// </summary>
    //[BaseAddress("http://10.115.63.135:4200/gateway/")]
    public class TrackLogProxyService : BaseProxyService//: IHttpDeclarative
    {
        private readonly string authHostUrl;
        private readonly ILogger<TrackLogProxyService> _logger;

        public TrackLogProxyService(IHttpRemoteService httpRemoteService, ILogger<TrackLogProxyService> logger) : base(httpRemoteService)
        {
            authHostUrl = App.GetConfig<string>("RemoteApi:DataAssetHostUrl");
            _logger = logger;
        }

        public override string GetToken()
        {
            var token = base.GetToken();
            if (token.IsNullOrWhiteSpace())
            {
                token = JwtHelper.GenerateJwtToken(new UserInfo()
                {
                    ChineseName = "Anonymous",
                    EnglishName = "Anonymous",
                    Department = "Anonymous",
                    Email = "Anny_Wu@jabil.com",
                    Id = "Anonymous",
                    Name = "Anonymous",
                    NtId = "Anonymous",
                    Surname = "Anonymous",
                    UserId = "Anonymous",
                    UserName = "Anonymous",
                    PhoneNumber = "",
                    Roles = new List<string>()
                });
            }
            return token;
        }

        /// <summary>
        /// 获取所有userInfo清单
        /// 实际请求接口：/gateway/homeapi/api/home/Common/GetUserToEmployeeModList
        /// http://cnhuam0itpoc81:4200/gateway/eureka/data/api/services/v1.0.0/HR/Base/tp_employee_base_info/query
        /// "x-token", "3b89b6aaa232a39461eb704f53656f74"
        /// </summary>
        /// <returns></returns>
        //[HttpGet("http://10.115.63.135:4200/gateway/homeapi/api/home/Common/GetUserToEmployeeModList")]
        public async Task TrackLog(ApiTrackLogInfo entity)
        {
            if (App.HttpContext.Items.TryGetValue("ApiTrackLog", out object logInfo))
            {
                var log = logInfo as ApiTrackLogInfo;
                entity.CallerDate = log.CallerDate;
                entity.ElapsedMilliseconds = (long)(DateTimeOffset.Now - log.CallerDate).TotalMilliseconds;
                entity.RequestParameters = log.RequestParameters;
            }
            try
            {
                var result = await httpRemoteService.PostAsAsync<Result>($"{authHostUrl}/api/TrackLog",
                  builder => builder.SetContent(entity, "application/json;charset=utf-8")
                  .AddAuthentication("Bearer", GetToken()));
                if (result==null||!result.Success)
                {
                    _logger.LogError($"TrackLogProxyService Post TrackLog Errory:[{result?.Code}]{result?.Msg}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TrackLogProxyService Post TrackLog Errory:[{ex.Message}]\r\n{ex.StackTrace}");
            }
        }

    }
}
