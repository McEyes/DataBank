using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAuditRecord;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowAuditRecord.Services;

namespace ITPortal.Flow.Application.FlowAuditRecord
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowAuditRecord/", Name = "流程审批记录服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowAuditRecordAppService : IDynamicApiController
    {
        private readonly IFlowAuditRecordService _FlowAuditRecordService;

        public FlowAuditRecordAppService(IFlowAuditRecordService dataApiService)
        {
            _FlowAuditRecordService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowAuditRecordEntity entity)
        {
            return await _FlowAuditRecordService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowAuditRecordService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowAuditRecordEntity> Get(Guid id)
        {
            return await _FlowAuditRecordService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowAuditRecordService.Delete(ids);
        }


        public async Task<FlowAuditRecordEntity> Single(FlowAuditRecordDto entity)
        {
            return await _FlowAuditRecordService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowAuditRecordEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowAuditRecordService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowAuditRecordEntity entity)
        {
            return await _FlowAuditRecordService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowAuditRecordEntity>> Page([FromQuery] FlowAuditRecordDto filter)
        {
            return await _FlowAuditRecordService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowAuditRecordEntity>> Page2(FlowAuditRecordDto filter)
        {
            return await _FlowAuditRecordService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowAuditRecordEntity>> Query([FromQuery] FlowAuditRecordDto entity)
        {
            return await _FlowAuditRecordService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowAuditRecordEntity>> Query2(FlowAuditRecordDto entity)
        {
            return await _FlowAuditRecordService.Query(entity);
        }

        #endregion base api
    }
}
