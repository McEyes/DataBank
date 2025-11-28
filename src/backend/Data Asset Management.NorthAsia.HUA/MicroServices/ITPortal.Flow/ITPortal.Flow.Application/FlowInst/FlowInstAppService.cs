using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowInst;
using ITPortal.Flow.Application.FlowInst.Dtos;
using ITPortal.Flow.Application.FlowInst.Services;
using ITPortal.Flow.Application.FlowTemplate.Dtos;
using ITPortal.Flow.Core;

namespace ITPortal.Flow.Application.FlowInst
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowInst/", Name = "流程实例服务")]
    [ApiDescriptionSettings(GroupName = "流程发起")]
    public class FlowInstAppService : IDynamicApiController
    {
        private readonly IFlowInstService _FlowInstService;

        public FlowInstAppService(IFlowInstService dataApiService)
        {
            _FlowInstService = dataApiService;
        }


        public async Task<FlowInstEntity> GetInfo(Guid flowId)
        {
            return await _FlowInstService.GetInfo(flowId);
        }

        //[HttpPost("Start")]
        //public async Task<ITPortal.Core.Services.IResult> StartFlowInst(StartFlowDto flowData)
        //{
        //    return await _FlowInstService.StartFlowInst(flowData);
        //}

        //public async Task<ITPortal.Core.Services.IResult> Approval(FlowAuditDto info)
        //{
        //    return await _FlowInstService.Approval(info);
        //}
        //public async Task<ITPortal.Core.Services.IResult> Reject(FlowAuditDto info)
        //{
        //    return await _FlowInstService.Reject(info);
        //}
        //[HttpPost("RejectStart")]
        //public async Task<ITPortal.Core.Services.IResult> RejectStart(FlowAuditDto info)
        //{
        //    return await _FlowInstService.RejectStart(info);
        //}
        //public async Task<ITPortal.Core.Services.IResult> Submit(FlowAuditDto info, FlowAction action)
        //{
        //    return await _FlowInstService.Submit(info, action);
        //}

        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowInstEntity entity)
        {
            return await _FlowInstService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowInstService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowInstEntity> Get(Guid id)
        {
            return await _FlowInstService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowInstService.Delete(ids);
        }


        public async Task<FlowInstEntity> Single(FlowInstDto entity)
        {
            return await _FlowInstService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowInstEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowInstService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowInstEntity entity)
        {
            return await _FlowInstService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowInstEntity>> Page([FromQuery] FlowInstDto filter)
        {
            return await _FlowInstService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowInstEntity>> Page2(FlowInstDto filter)
        {
            return await _FlowInstService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowInstEntity>> Query([FromQuery] FlowInstDto entity)
        {
            return await _FlowInstService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowInstEntity>> Query2(FlowInstDto entity)
        {
            return await _FlowInstService.Query(entity);
        }

        #endregion base api
    }
}
