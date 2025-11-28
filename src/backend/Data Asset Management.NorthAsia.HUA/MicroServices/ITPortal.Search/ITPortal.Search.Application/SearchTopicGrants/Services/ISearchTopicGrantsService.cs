using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopicGrants.Dtos;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.SearchTopicGrants.Services
{
    public interface ISearchTopicGrantsService : IBaseService<SearchTopicGrantsEntity, SearchTopicGrantsDto, Guid>
    {
        Task<List<SearchTopicGrantsResultDto>> GetGrantsByRoleId(string roleId);
        Task<List<Guid>> GetTopicIdsByRoleIds(List<string> roleIds);
        Task<Result> Grants(SearchTopicGrantsDto grants);
    }
}
