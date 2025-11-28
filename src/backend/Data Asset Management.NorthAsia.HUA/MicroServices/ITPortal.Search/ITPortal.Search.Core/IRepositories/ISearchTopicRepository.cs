using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Jabil.Service.Extension.Permissions.MFGTranningPermissions;
using Volo.Abp.Domain.Repositories;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Core.IRepositories
{
    public interface ISearchTopicRepository : IRepository<SearchTopic, Guid>
    {

        Task<SearchTopic> Create(SearchTopic topic);

        Task<SearchTopic> Update(SearchTopic topic);

        Task<bool> Delete(Guid id);

        Task<SearchTopic> GetByNameOrTopic(string name, string topic);

        Task<SearchTopic> GetByTopic(string topic);

        Task<List<SearchTopic>> GetByTopics(List<string> topics);

        Task<(IEnumerable<SearchTopic> items, long totalCount)> GetByTopicIds(List<Guid> topicIds, int pageIndex = -1, int pageSize = -1);

        Task<(IEnumerable<SearchTopic> items, long totalCount)> GetListByPage(string keyword, int pageIndex, int pageSize);


    }
}
