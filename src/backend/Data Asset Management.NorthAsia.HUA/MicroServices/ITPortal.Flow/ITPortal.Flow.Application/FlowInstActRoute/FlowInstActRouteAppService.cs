using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowInstActRoute;
using ITPortal.Flow.Application.FlowInstActRoute.Dtos;
using ITPortal.Flow.Application.FlowInstActRoute.Services;

namespace ITPortal.Flow.Application.FlowInstActRoute
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowInstActRoute/", Name = "流程实例节点路由服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowInstActRouteAppService : IDynamicApiController
    {
        private readonly IFlowInstActRouteService _FlowInstActRouteService;

        public FlowInstActRouteAppService(IFlowInstActRouteService dataApiService)
        {
            _FlowInstActRouteService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowInstActRouteEntity entity)
        {
            return await _FlowInstActRouteService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowInstActRouteService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowInstActRouteEntity> Get(Guid id)
        {
            return await _FlowInstActRouteService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowInstActRouteService.Delete(ids);
        }


        public async Task<FlowInstActRouteEntity> Single(FlowInstActRouteDto entity)
        {
            return await _FlowInstActRouteService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowInstActRouteEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowInstActRouteService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowInstActRouteEntity entity)
        {
            return await _FlowInstActRouteService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowInstActRouteEntity>> Page([FromQuery] FlowInstActRouteDto filter)
        {
            return await _FlowInstActRouteService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowInstActRouteEntity>> Page2(FlowInstActRouteDto filter)
        {
            return await _FlowInstActRouteService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowInstActRouteEntity>> Query([FromQuery] FlowInstActRouteDto entity)
        {
            return await _FlowInstActRouteService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowInstActRouteEntity>> Query2(FlowInstActRouteDto entity)
        {
            return await _FlowInstActRouteService.Query(entity);
        }

        #endregion base api
    }
}
