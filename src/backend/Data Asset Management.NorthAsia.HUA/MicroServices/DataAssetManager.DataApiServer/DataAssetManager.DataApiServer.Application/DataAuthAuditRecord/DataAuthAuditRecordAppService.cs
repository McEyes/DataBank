using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/AuditRecord/", Name = "数据资产DataAuthAuditRecord服务")]
    [ApiDescriptionSettings(GroupName = "数据资产DataAuthAuditRecord")]
    public class DataAuthAuditRecordAppService : IDynamicApiController
    {
        private readonly IDataAuthAuditRecordService _dataAuthAuditRecordService;
        private readonly IEmployeeBaseInfoService _userService;
        private readonly IDataApiService _dataApiService;
        private readonly IDistributedCacheService _cache;

        public DataAuthAuditRecordAppService(IDataAuthAuditRecordService dataApiService, IDataApiService dataApiService1, IEmployeeBaseInfoService userProxyService, IDistributedCacheService cache)
        {
            _dataAuthAuditRecordService = dataApiService;
          _userService = userProxyService;
            _cache = cache;
            _dataApiService = dataApiService1;
        }

        [HttpGet("GetUsers")]
        public async Task<List<UserInfo>> GetUsersAsync()
        {
            return (await _userService.GetUsersAsync()).Data;
        }

        //public async Task<Result<JabusEmployeeInfo>> GetEmployeeAsync(string ntid)
        //{
        //    return await _dataAuthAuditRecordService.GetEmployeeAsync(ntid);
        //}

        ///// <summary>
        ///// 权限流程申请
        ///// </summary>
        ///// <param name="authApplyDto"></param>
        ///// <returns></returns>
        //[HttpPost("applyAuth")]
        //public async Task<string> ApplyAuth(DataAuthApply authApplyDto)
        //{
        //    var result = await _dataAuthAuditRecordService.ApplyAuth(authApplyDto);
        //    if (result.Success)
        //        return string.Empty;
        //    return result.Msg.ToString();
        //}

        ///// <summary>
        ///// 检查权限流程，异常返回已经拥有的权限信息
        ///// </summary>
        ///// <param name="authApplyDto"></param>
        ///// <returns></returns>
        //[HttpPost("checkAuth")]
        //public async Task<ITPortal.Core.Services.IResult> CheckAuth(DataAuthApply authApplyDto)
        //{
        //    return await _dataAuthAuditRecordService.CheckAuth(authApplyDto);
        //}


        //[HttpPost("authBack")]
        //public async Task<ITPortal.Core.Services.IResult> AuthBack(AuthResultDto authResultDto)
        //{
        //    return await _dataAuthAuditRecordService.UpdateAuth(authResultDto);
        //}


        #region base api


        [HttpPost()]
        public async Task<int> Create(DataAuthAuditRecordEntity entity)
        {
            return await _dataAuthAuditRecordService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataAuthAuditRecordService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataAuthAuditRecordEntity> Get(string id)
        {
            return await _dataAuthAuditRecordService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataAuthAuditRecordService.Delete(ids);
        }


        public async Task<DataAuthAuditRecordEntity> Single(DataAuthAuditRecordDto entity)
        {
            return await _dataAuthAuditRecordService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(DataAuthAuditRecordEntity entity)
        {
            return await _dataAuthAuditRecordService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataAuthAuditRecordEntity entity)
        {
            return await _dataAuthAuditRecordService.ModifyHasChange(entity);
        }

        [HttpPost("page")]
        public async Task<PageResult<DataAuthAuditRecordEntity>> Page(DataAuthAuditRecordDto filter)
        {
            return await _dataAuthAuditRecordService.Page(filter);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataAuthAuditRecordEntity>> Page2([FromQuery] DataAuthAuditRecordDto filter)
        {
            return await _dataAuthAuditRecordService.Page(filter);
        }

        [HttpPost("list")]
        public async Task<List<DataAuthAuditRecordEntity>> Query(DataAuthAuditRecordDto entity)
        {
            return await _dataAuthAuditRecordService.Query(entity);
        }

        [HttpGet("list")]
        public async Task<List<DataAuthAuditRecordEntity>> Query2([FromQuery] DataAuthAuditRecordDto entity)
        {
            return await _dataAuthAuditRecordService.Query(entity);
        }

        #endregion base api
    }
}
