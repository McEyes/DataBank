using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTemplate;
using ITPortal.Flow.Application.FlowTemplate.Dtos;
using ITPortal.Flow.Application.FlowTemplate.Services;

using static Grpc.Core.Metadata;

namespace ITPortal.Flow.Application.FlowTemplate
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/FlowTemplate/", Name = "流程模板服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class FlowTemplateAppService : IDynamicApiController
    {
        private readonly IFlowTemplateService _FlowTemplateService;

        public FlowTemplateAppService(IFlowTemplateService dataApiService)
        {
            _FlowTemplateService = dataApiService;
        }

        [HttpGet("Info/name/{tempName}")]
        public async Task<FlowTemplateDto> GetInfo(string tempName)
        {
            return await _FlowTemplateService.GetTempInfo(tempName);
        }

        [HttpGet("Info/{tempId}")]
        public async Task<FlowTemplateDto> GetInfo(Guid tempId)
        {
            return await _FlowTemplateService.GetTempInfo(tempId);
        }
        [HttpGet("Check/{tempId}")]
        public async Task<Result<string>> CheckTempRoute(Guid tempId)
        {
            return await _FlowTemplateService.CheckTempRoute(tempId);
        }


        #region base api


        [HttpPost()]
        public async Task<int> Create(FlowTemplateEntity entity)
        {
            return await _FlowTemplateService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowTemplateService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<FlowTemplateEntity> Get(Guid id)
        {
            return await _FlowTemplateService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _FlowTemplateService.Delete(ids);
        }


        public async Task<FlowTemplateEntity> Single(FlowTemplateDto entity)
        {
            return await _FlowTemplateService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, FlowTemplateEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _FlowTemplateService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(FlowTemplateEntity entity)
        {
            return await _FlowTemplateService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<FlowTemplateEntity>> Page([FromQuery] FlowTemplateDto filter)
        {
            return await _FlowTemplateService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<FlowTemplateEntity>> Page2(FlowTemplateDto filter)
        {
            return await _FlowTemplateService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<FlowTemplateEntity>> Query([FromQuery] FlowTemplateDto entity)
        {
            return await _FlowTemplateService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<FlowTemplateEntity>> Query2(FlowTemplateDto entity)
        {
            return await _FlowTemplateService.Query(entity);
        }

        #endregion base api
    }
}
