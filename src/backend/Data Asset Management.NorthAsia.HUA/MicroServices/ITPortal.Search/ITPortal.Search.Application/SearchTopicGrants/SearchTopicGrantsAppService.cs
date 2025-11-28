using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopicGrants.Dtos;
using ITPortal.Search.Core.Models;

using jb.smartchangeover.Service.Domain.Shared.DistributedCache;

namespace ITPortal.Search.Application.SearchTopicGrants.Services
{
    /// <summary>
    /// 问题反馈api集合
    /// </summary>
    //[AppAuthorize]
    [Route("api/SearchTopicGrants/", Name = "全局搜索 SearchTopicGrants服务")]
    [ApiDescriptionSettings(GroupName = "全局搜索 SearchTopicGrants")]
    public class SearchTopicGrantsAppService : IDynamicApiController
    {
        private readonly ISearchTopicGrantsService _searchTopicGrantsService;
        private readonly IDistributedCacheService _cache;

        public SearchTopicGrantsAppService(ISearchTopicGrantsService searchTopicGrantsService, IDistributedCacheService cache)
        {
            _searchTopicGrantsService = searchTopicGrantsService;
            _cache = cache;
        }

        public async Task<List<Guid>> GetTopicIdsByRoleIds(List<Guid> roleIds)
        {
            return await _searchTopicGrantsService.GetTopicIdsByRoleIds(roleIds);
        }

        public async Task<List<SearchTopicGrantsResultDto>> GetGrantsByRoleId(Guid roleId)
        {
            return await _searchTopicGrantsService.GetGrantsByRoleId(roleId);
        }

        public async Task<Result> Grants(SearchTopicGrantsDto grants)
        {
            return await _searchTopicGrantsService.Grants(grants);
        }
    }
}
