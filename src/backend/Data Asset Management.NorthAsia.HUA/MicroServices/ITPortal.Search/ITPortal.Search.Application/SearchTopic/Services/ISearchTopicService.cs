using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopic.Dtos;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.SearchTopic.Services
{
    public interface ISearchTopicService : IBaseService<SearchTopicEntity, SearchTopicDto, Guid>
    {
        Task<SearchTopicEntity> GetByNameOrTopic(string name, string topic);
        Task<SearchTopicEntity> GetByTopic(string topic);
        Task<(IEnumerable<SearchTopicEntity> items, long totalCount)> GetByTopicIds(List<Guid> topicIds, int pageIndex = -1, int pageSize = -1);
        Task<List<SearchTopicEntity>> GetByTopics(List<string> topics);
        Task<PageResult<SearchTopicDto>> GetListByPage(PageEntity<Guid> filter);

    }
}
