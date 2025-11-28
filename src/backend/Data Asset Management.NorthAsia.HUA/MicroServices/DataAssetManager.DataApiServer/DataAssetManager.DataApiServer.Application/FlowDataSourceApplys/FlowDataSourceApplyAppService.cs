using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataApiServer.Application.FlowDataSourceApply.Services;
using DataAssetManager.DataApiServer.Application.FlowDataSourceApplys.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

using MapsterMapper;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/DataSourceApply/", Name = "数据资产 DataSourceApply 服务")]
    [ApiDescriptionSettings(GroupName = "数据资产 数据源申请")]
    public class FlowDataSourceApplyAppService : IDynamicApiController
    {
        private readonly IFlowDataSourceApplyService _dataService;
        private readonly IDistributedCacheService _cache;

        public FlowDataSourceApplyAppService(IFlowDataSourceApplyService dataApiService,IDistributedCacheService cache)
        {
            _dataService = dataApiService;
            _cache = cache;
        }


        /// <summary>
        /// 权限流程申请,
        /// public & internal级别数据自动审批
        /// </summary>
        /// <param name="authApplyDto"></param>
        /// <returns></returns>
        [HttpPost("ApplyAuth")]
        public async Task<ITPortal.Core.Services.IResult> ApplyAuth(FlowSourceApply authApplyDto)
        {
            return await _dataService.ApplyAuth(authApplyDto);
        }


        /// <summary>
        /// 流程回写状态
        /// </summary>
        /// <param name="authResultDto"></param>
        /// <returns></returns>
        [HttpPost("CallBack")]
        public async Task<string> CallBack(Result<FlowBackDataEntity> authResultDto)
        {
            var result = await _dataService.CallBack(authResultDto);
            if (result.Success)
                return result.Data;
            else return result.Msg?.ToString();
        }



        #region base api


        [HttpPost()]
        public async Task<int> Post(FlowDataSourceApplyEntity entity)
        {
            return await _dataService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _dataService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowDataSourceApplyEntity> Get(Guid id)
        {
            return await _dataService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(dynamic[] ids)
        {
            return await _dataService.Delete(ids);
        }


        public async Task<FlowDataSourceApplyEntity> Single(FlowDataSourceApplyDto entity)
        {
            return await _dataService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(FlowDataSourceApplyEntity entity)
        {
            return await _dataService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowDataSourceApplyEntity entity)
        {
            return await _dataService.ModifyHasChange(entity);
        }

        [HttpPost("page")]
        public async Task<PageResult<FlowDataSourceApplyEntity>> Page(FlowDataSourceApplyDto filter)
        {
            return await _dataService.Page(filter);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowDataSourceApplyEntity>> Page3([FromQuery] FlowDataSourceApplyDto filter)
        {
            return await _dataService.Page(filter);
        }

        [HttpPost("list")]
        public async Task<List<FlowDataSourceApplyEntity>> Query(FlowDataSourceApplyDto entity)
        {
            return await _dataService.Query(entity);
        }

        [HttpGet("list")]
        public async Task<List<FlowDataSourceApplyEntity>> Query3([FromQuery] FlowDataSourceApplyDto entity)
        {
            return await _dataService.Query(entity);
        }

        #endregion base api
    }
}
