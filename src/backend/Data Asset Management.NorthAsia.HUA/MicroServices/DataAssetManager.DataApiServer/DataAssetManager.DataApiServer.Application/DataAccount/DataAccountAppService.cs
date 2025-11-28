using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using ITPortal.Core;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Extension.System;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using Furion.EventBus;

namespace DataAssetManager.DataApiServer.Application.DataAsset
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/accounts/", Name = "数据资产系统服务")]
    [ApiDescriptionSettings(GroupName = "数据资产Api")]
    public class DataAccountAppService : IDynamicApiController
    {
        private readonly IDataApiService _dataApiService;
        private readonly IEmployeeBaseInfoService _employeeService;
        private readonly IEventPublisher _eventPublisher;
        public DataAccountAppService(IDataApiService dataApiService, IEmployeeBaseInfoService employeeProxyService, IEventPublisher eventPublisher)
        {
            _dataApiService = dataApiService;
            _employeeService = employeeProxyService;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// 检查config参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public dynamic GetConfig(string key = "RemoteApi:AppHostUrl")
        {
            return App.GetConfig<string>(key);
        }

        #region dynamic api

        ///// <summary>
        ///// 注册数据资产API
        ///// </summary>
        ///// <param name="apiConfig"></param>
        ///// <returns></returns>
        //public void Register(RouteInfo apiConfig)
        //{
        //    _dataApiService.Register(apiConfig);
        //}

        ///// <summary>
        ///// 注销数据资产API
        ///// </summary>
        ///// <param name="apiUrl"></param>
        ///// <returns></returns>
        //public dynamic Unregister(string apiUrl)
        //{
        //    return _dataApiService.Unregister(apiUrl);
        //}
        [HttpGet("InitRoutes")]
        public async Task<string> InitRoutes(bool clearCache = false)
        {
            return await _dataApiService.InitRoutes(clearCache);
        }

        [HttpGet("AllUser")]
        public async Task<List<UserInfo>> AllUserAsync(string ntid = "", bool clearCache = false)
        {
            var result = await _employeeService.GetUsersAsync(clearCache);
            if (result.Success) return result.Data.WhereIF(ntid.IsNotNullOrWhiteSpace(), f => f.NtId == ntid).ToList();
            else throw new AppFriendlyException(result.Msg.ToString(), result.Code);
        }

        [HttpGet("AllEmployee")]
        public async Task<List<JabusEmployeeInfo>> AllEmployeeAsync(string ntid = "", bool clearCache = false)
        {
            var result = await _employeeService.GetAllEmployee(clearCache);
            return result?.WhereIF(ntid.IsNotNullOrWhiteSpace(), f => f.WorkNTID == ntid).ToList();
        }

        /// <summary>
        /// PublishEvenBus
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("PublishEvenBus")]
        public async Task PublishEvenBus(string eventId = "")
        {
            if (eventId.IsNullOrWhiteSpace()) eventId = DataAssetManagerConst.DataTable_UserHashKey;
            await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(eventId));
        }

        ///// <summary>
        ///// 生成token
        ///// </summary>
        ///// <param name="userid"></param>
        ///// <param name="username"></param>
        ///// <returns></returns>
        //[AllowAnonymous]
        //public string GetToken(string userid, string username)
        //{
        //    // 生成 token

        //    var accessToken = JwtHelper.GenerateJwtToken(new UserInfo() { UserId = userid, UserName = username });
        //    return accessToken;
        //}
        ///// <summary>
        ///// 验证token
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[AllowAnonymous]
        //public dynamic ValidateToken(string token)
        //{
        //    var accessToken = JwtHelper.ValidateToken(token);
        //    return new UserInfo(accessToken);
        //}


        /// <summary>
        /// 验证token
        /// </summary>
        /// <returns></returns>
        [HttpGet("UserInfo")]
        public dynamic GetUserInfo()
        {
            return App.HttpContext.GetCurrUserInfo() ?? new UserInfo();
        }
        #endregion dynamic api


    }
}
