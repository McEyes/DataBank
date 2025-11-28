using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAttachments.Dtos;
using ITPortal.Flow.Application.FlowAttachments.Services;

namespace ITPortal.Flow.Application.FlowAttachment
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/Attacchment/", Name = "流程附件")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowAttachmentAppService : IDynamicApiController
    {
        private readonly IFlowAttachmentService _FlowAttachmentService;

        public FlowAttachmentAppService(IFlowAttachmentService dataApiService)
        {
            _FlowAttachmentService = dataApiService;
        }




        /// <summary>
        /// 获取流程模板配置的必填附件
        /// </summary>
        /// <param name="tampName"></param>
        /// <returns></returns>
        [HttpGet("FlowTemp/{tampName}")]
        public async Task<FlowAttachmentEntity> GetTempAttachList(Guid tampName)
        {
            return await _FlowAttachmentService.Get(tampName);
        }




        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowAttachmentEntity entity)
        {
            return await _FlowAttachmentService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowAttachmentService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowAttachmentEntity> Get(Guid id)
        {
            return await _FlowAttachmentService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowAttachmentService.Delete(ids);
        }


        public async Task<FlowAttachmentEntity> Single(FlowAttachmentDto entity)
        {
            return await _FlowAttachmentService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowAttachmentEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowAttachmentService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowAttachmentEntity entity)
        {
            return await _FlowAttachmentService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowAttachmentEntity>> Page([FromQuery] FlowAttachmentDto filter)
        {
            return await _FlowAttachmentService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowAttachmentEntity>> Page2(FlowAttachmentDto filter)
        {
            return await _FlowAttachmentService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowAttachmentEntity>> Query([FromQuery] FlowAttachmentDto entity)
        {
            return await _FlowAttachmentService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowAttachmentEntity>> Query2(FlowAttachmentDto entity)
        {
            return await _FlowAttachmentService.Query(entity);
        }

        #endregion base api
    }
}
