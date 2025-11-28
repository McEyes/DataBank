using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/AuditApply/", Name = "数据资产DataAuthApply服务")]
    [ApiDescriptionSettings(GroupName = "数据资产DataAuthApply")]
    public class DataAuthApplyAppService : IDynamicApiController
    {
        private readonly IDataAuthApplyService _dataAuthApplyService;
        private readonly IAssetClientsService _assetClientsService;
        private readonly ILogger<DataAuthApplyAppService> _logger;

        public DataAuthApplyAppService(IDataAuthApplyService dataAuthApplyService,
            //IDataApiService dataApiService,
            IAssetClientsService assetClientsService,
            ILogger<DataAuthApplyAppService> logger)
        {
            _logger = logger;
            _dataAuthApplyService = dataAuthApplyService;
            _assetClientsService = assetClientsService;
        }

        //[HttpGet("GetUsers")]
        //public async Task<List<JabusUserInfo>> GetUsersAsync()
        //{
        //    return (await _userService.GetUsersAsync()).Data;
        //}

        //public async Task<Result<JabusEmployeeInfo>> GetEmployeeAsync(string ntid)
        //{
        //    return await _DataAuthApplyService.GetEmployeeAsync(ntid);
        //}

        /// <summary>
        /// 权限流程申请,
        /// public & internal级别数据自动审批
        /// </summary>
        /// <param name="authApplyDto"></param>
        /// <returns></returns>
        [HttpPost("ApplyAuth")]
        public async Task<ITPortal.Core.Services.IResult> ApplyAuth(DataAuthApplyInfo authApplyDto)
        {
            return await _dataAuthApplyService.ApplyAuth(authApplyDto);
        }

        /// <summary>
        /// 检查权限流程，异常返回已经拥有的权限信息
        /// </summary>
        /// <param name="authApplyDto"></param>
        /// <returns></returns>
        [HttpPost("checkauth")]
        public async Task<Result<List<AssetClientScopesEntity>>> CheckAuth(DataAuthCheckDto authApplyDto)
        {
            return await _assetClientsService.CheckAuth(authApplyDto);
        }


        /// <summary>
        /// 流程回写状态
        /// </summary>
        /// <param name="flowCallBackData"></param>
        /// <returns></returns>
        [HttpPost("AuthBack")]
        public async Task<string> AuthBack(Result<ITPortal.Core.ProxyApi.Flow.Dto.FlowBackDataEntity> flowCallBackData)
        {
            _logger.LogInformation($"Process approval callback parameters:{flowCallBackData.ToJSON()}");
            var msg = await _dataAuthApplyService.AuthBack(flowCallBackData);
            return msg;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(DataAuthApplyEntity entity)
        {
            return await _dataAuthApplyService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _dataAuthApplyService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataAuthApplyEntity> Get(Guid id)
        {
            return await _dataAuthApplyService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(dynamic[] ids)
        {
            return await _dataAuthApplyService.Delete(ids);
        }


        public async Task<DataAuthApplyEntity> Single(DataAuthApplyDto entity)
        {
            return await _dataAuthApplyService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(DataAuthApplyEntity entity)
        {
            return await _dataAuthApplyService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataAuthApplyEntity entity)
        {
            return await _dataAuthApplyService.ModifyHasChange(entity);
        }

        [HttpPost("Page")]
        public async Task<PageResult<DataAuthApplyEntity>> Page(DataAuthApplyDto filter)
        {
            return await _dataAuthApplyService.Page(filter);
        }

        [HttpGet("Page")]
        public async Task<PageResult<DataAuthApplyEntity>> PageQuery([FromQuery] DataAuthApplyDto filter)
        {
            return await _dataAuthApplyService.Page(filter);
        }

        [HttpPost("List")]
        public async Task<List<DataAuthApplyEntity>> Query(DataAuthApplyDto entity)
        {
            return await _dataAuthApplyService.Query(entity);
        }

        [HttpGet("List")]
        public async Task<List<DataAuthApplyEntity>> List([FromQuery] DataAuthApplyDto entity)
        {
            return await _dataAuthApplyService.Query(entity);
        }

        #endregion base api
    }
}
