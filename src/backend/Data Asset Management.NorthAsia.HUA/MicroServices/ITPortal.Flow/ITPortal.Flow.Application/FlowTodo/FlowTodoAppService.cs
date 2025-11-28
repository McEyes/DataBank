using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTodo;
using ITPortal.Flow.Application.FlowTodo.Dtos;
using ITPortal.Flow.Application.FlowTodo.Services;

namespace ITPortal.Flow.Application.FlowTodo
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowTodo/", Name = "流程待办服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowTodoAppService : IDynamicApiController
    {
        private readonly IFlowTodoService _FlowTodoService;

        public FlowTodoAppService(IFlowTodoService dataApiService)
        {
            _FlowTodoService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowTodoEntity entity)
        {
            return await _FlowTodoService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowTodoService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowTodoEntity> Get(Guid id)
        {
            return await _FlowTodoService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowTodoService.Delete(ids);
        }


        public async Task<FlowTodoEntity> Single(FlowTodoDto entity)
        {
            return await _FlowTodoService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowTodoEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowTodoService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowTodoEntity entity)
        {
            return await _FlowTodoService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowTodoEntity>> Page([FromQuery] FlowTodoDto filter)
        {
            return await _FlowTodoService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowTodoEntity>> Page2(FlowTodoDto filter)
        {
            return await _FlowTodoService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowTodoEntity>> Query([FromQuery] FlowTodoDto entity)
        {
            return await _FlowTodoService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowTodoEntity>> Query2(FlowTodoDto entity)
        {
            return await _FlowTodoService.Query(entity);
        }

        #endregion base api
    }
}
