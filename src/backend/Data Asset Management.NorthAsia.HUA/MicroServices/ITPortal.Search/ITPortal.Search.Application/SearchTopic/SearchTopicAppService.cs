using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopic.Dtos;
using ITPortal.Search.Application.SearchTopicGrants.Dtos;
using ITPortal.Search.Application.SearchTopicGrants.Services;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.SearchTopic.Services
{
    /// <summary>
    /// 问题反馈api集合
    /// </summary>
    [AppAuthorize]
    [Route("api/SearchTopic/", Name = "全局搜索 SearchTopic服务")]
    [ApiDescriptionSettings(GroupName = "全局搜索 SearchTopic")]
    public class SearchTopicAppService : IDynamicApiController
    {
        private readonly ISearchTopicService _searchTopicService;
        private readonly ISearchTopicGrantsService _searchTopicGrantsService;

        public SearchTopicAppService(ISearchTopicService searchTopicService,ISearchTopicGrantsService searchTopicGrantsService)
        {
            _searchTopicService = searchTopicService;
            _searchTopicGrantsService = searchTopicGrantsService;
        }

        /// <summary>
        /// 创建主题
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpPost]
        //[SwaggerOperation(summary: "创建主题")]
        public async Task<int> Create(SearchTopicDto topic)
        {
            return await _searchTopicService.Create(topic.Adapt<SearchTopicEntity>());
        }
        /// <summary>
        /// 删除主题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        //[SwaggerOperation(summary: "删除主题")]
        public async Task<bool> Delete(Guid id)
        {
            return await _searchTopicService.Delete(id);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("List")]
        //[SwaggerOperation(summary: "分页查询")]
        public async Task<PageResult<SearchTopicDto>> GetListByPage(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            return await _searchTopicService.GetListByPage(new PageEntity<Guid>() { Keyword = keyword, PageNum = pageIndex, PageSize = pageSize });
        }
        /// <summary>
        /// 更新主题
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpPut]
        //[SwaggerOperation(summary: "更新主题")]
        public async Task<int> Update(SearchTopicDto topic)
        {
            return await _searchTopicService.Modify(topic.Adapt<SearchTopicEntity>());
        }
        /// <summary>
        /// 授权角色主题
        /// </summary>
        /// <param name="grants"></param>
        /// <returns></returns>
        [HttpPost("Grants")]
        //[SwaggerOperation(summary: "授权角色主题")]
        public async Task Grants(SearchTopicGrantsDto grants)
        {
          await  _searchTopicGrantsService.Grants(grants);
        }
        /// <summary>
        /// 根据角色ID获取所有主题
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("Grants/{roleId}")]
        //[SwaggerOperation(summary: "根据角色ID获取所有主题")]
        public Task<List<SearchTopicGrantsResultDto>> GetGrantsByRoleId(string roleId)
        {
            return _searchTopicGrantsService.GetGrantsByRoleId(roleId);
        }
    }
}
