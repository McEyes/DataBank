using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempActRoute;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;
using ITPortal.Flow.Application.FlowTempActRoute.Services;

namespace ITPortal.Flow.Application.FlowTempActRoute
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowTempActRoute/", Name = "流程模板节点路由服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowTempActRouteAppService : IDynamicApiController
    {
        private readonly IFlowTempActRouteService _FlowTempActRouteService;

        public FlowTempActRouteAppService(IFlowTempActRouteService dataApiService)
        {
            _FlowTempActRouteService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowTempActRouteEntity entity)
        {
            return await _FlowTempActRouteService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowTempActRouteService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowTempActRouteEntity> Get(Guid id)
        {
            return await _FlowTempActRouteService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowTempActRouteService.Delete(ids);
        }


        public async Task<FlowTempActRouteEntity> Single(FlowTempActRouteDto entity)
        {
            return await _FlowTempActRouteService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowTempActRouteEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowTempActRouteService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowTempActRouteEntity entity)
        {
            return await _FlowTempActRouteService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowTempActRouteEntity>> Page([FromQuery] FlowTempActRouteDto filter)
        {
            return await _FlowTempActRouteService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowTempActRouteEntity>> Page2(FlowTempActRouteDto filter)
        {
            return await _FlowTempActRouteService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowTempActRouteEntity>> Query([FromQuery] FlowTempActRouteDto entity)
        {
            return await _FlowTempActRouteService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowTempActRouteEntity>> Query2(FlowTempActRouteDto entity)
        {
            return await _FlowTempActRouteService.Query(entity);
        }

        #endregion base api
    }
}
