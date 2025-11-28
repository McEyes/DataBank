
using ITPortal.Core.Repositorys;
using ITPortal.Search.Core.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITPortal.Search.Core.IRepositories
{
    public interface ISearchTopicGrantsRepository : Repository<SearchTopicGrants>
    {

        /// <summary>
        /// 角色授权Topic
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="topicIds"></param>
        /// <returns></returns>
        Task<List<Guid>> Grants(Guid roleId, List<Guid> topicIds);

        /// <summary>
        /// 根据角色ID集合获取TopicID集合
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        Task<List<Guid>> GetTopicIdsByRoleIds(List<Guid> roleIds);

    }
}
