using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowActInst.Dtos;

namespace ITPortal.Flow.Application.FlowActInst
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowActInst/", Name = "流程节点实例服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowActInstAppService : IDynamicApiController
    {
        private readonly IFlowActInstService _flowActInstService;

        public FlowActInstAppService(IFlowActInstService dataApiService)
        {
            _flowActInstService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowActInstEntity entity)
        {
            return await _flowActInstService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _flowActInstService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowActInstEntity> Get(Guid id)
        {
            return await _flowActInstService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _flowActInstService.Delete(ids);
        }


        public async Task<FlowActInstEntity> Single(FlowActInstDto entity)
        {
            return await _flowActInstService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowActInstEntity entity)
        {
            if (id!=Guid.Empty) entity.Id = id;
            return await _flowActInstService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowActInstEntity entity)
        {
            return await _flowActInstService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowActInstEntity>> Page([FromQuery]FlowActInstDto filter)
        {
            return await _flowActInstService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowActInstEntity>> Page2(FlowActInstDto filter)
        {
            return await _flowActInstService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowActInstEntity>> Query([FromQuery] FlowActInstDto entity)
        {
            return await _flowActInstService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowActInstEntity>> Query2(FlowActInstDto entity)
        {
            return await _flowActInstService.Query(entity);
        }

        #endregion base api
    }
}
