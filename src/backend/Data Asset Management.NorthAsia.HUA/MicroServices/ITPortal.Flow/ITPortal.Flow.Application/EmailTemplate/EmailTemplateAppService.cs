using ITPortal.Core.Services;
using ITPortal.Flow.Application.EmailTemplate.Dtos;

namespace ITPortal.Flow.Application.EmailTemplate
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/EmailTemplate/", Name = "流程 EmailTemplate 服务")]
    [ApiDescriptionSettings(GroupName = "流程")]
    public class EmailTemplateAppService : IDynamicApiController
    {
        private readonly IEmailTemplateService _flowActInstService;

        public EmailTemplateAppService(IEmailTemplateService dataApiService)
        {
            _flowActInstService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(EmailTemplateEntity entity)
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
        public async Task<EmailTemplateEntity> Get(Guid id)
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


        public async Task<EmailTemplateEntity> Single(EmailTemplateDto entity)
        {
            return await _flowActInstService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, EmailTemplateEntity entity)
        {
            if (id!=Guid.Empty) entity.Id = id;
            return await _flowActInstService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(EmailTemplateEntity entity)
        {
            return await _flowActInstService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<EmailTemplateEntity>> Page([FromQuery]EmailTemplateDto filter)
        {
            return await _flowActInstService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<EmailTemplateEntity>> Page2(EmailTemplateDto filter)
        {
            return await _flowActInstService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<EmailTemplateEntity>> Query([FromQuery] EmailTemplateDto entity)
        {
            return await _flowActInstService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<EmailTemplateEntity>> Query2(EmailTemplateDto entity)
        {
            return await _flowActInstService.Query(entity);
        }

        #endregion base api
    }
}
