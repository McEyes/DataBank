using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempAct;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Services;

namespace ITPortal.Flow.Application.FlowTempAct
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowTempAct/", Name = "流程模板节点服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowTempActAppService : IDynamicApiController
    {
        private readonly IFlowTempActService _FlowTempActService;

        public FlowTempActAppService(IFlowTempActService dataApiService)
        {
            _FlowTempActService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowTempActEntity entity)
        {
            return await _FlowTempActService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowTempActService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowTempActEntity> Get(Guid id)
        {
            return await _FlowTempActService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowTempActService.Delete(ids);
        }


        public async Task<FlowTempActEntity> Single(FlowTempActDto entity)
        {
            return await _FlowTempActService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowTempActEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowTempActService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowTempActEntity entity)
        {
            return await _FlowTempActService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowTempActEntity>> Page([FromQuery] FlowTempActDto filter)
        {
            return await _FlowTempActService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowTempActEntity>> Page2(FlowTempActDto filter)
        {
            return await _FlowTempActService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowTempActEntity>> Query([FromQuery] FlowTempActDto entity)
        {
            return await _FlowTempActService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowTempActEntity>> Query2(FlowTempActDto entity)
        {
            return await _FlowTempActService.Query(entity);
        }

        #endregion base api
    }
}
