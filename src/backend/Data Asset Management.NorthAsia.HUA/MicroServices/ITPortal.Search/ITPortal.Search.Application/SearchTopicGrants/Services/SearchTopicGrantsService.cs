using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Search.Application.SearchTopicGrants.Dtos;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.SearchTopicGrants.Services
{
    public class SearchTopicGrantsService : BaseService<SearchTopicGrantsEntity, SearchTopicGrantsDto, Guid>, ISearchTopicGrantsService, ITransient
    {

        public SearchTopicGrantsService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache,true)
        {
        }

        public override ISugarQueryable<SearchTopicGrantsEntity> BuildFilterQuery(SearchTopicGrantsDto filter)
        {
            return CurrentDb.Queryable<SearchTopicGrantsEntity>()
               .WhereIF(filter.RoleId.IsNotNullOrWhiteSpace(), f => f.RoleId == filter.RoleId) 
               .OrderByDescending(f => f.CreateTime);
        }

        public async Task<List<Guid>> GetTopicIdsByRoleIds(List<string> roleIds)
        {
            if (roleIds.Count == 0) return new List<Guid>();

            return await AsQueryable().Where(p => roleIds.Contains(p.RoleId))
                .Select(p => p.TopicId)
                .ToListAsync();
        }
        public async Task<List<SearchTopicGrantsResultDto>> GetGrantsByRoleId(string roleId)
        {
            var topicIds = await GetTopicIdsByRoleIds(new List<string> { roleId });

            var topics = await AsQueryable<SearchTopicEntity>().Where(p => p.IsEnable).ToListAsync();

            var list = topics.Adapt<List<SearchTopicGrantsResultDto>>();

            foreach (var item in list)
            {
                item.IsGrant = topicIds.Any(tid => tid.Equals(item.Id));
            }

            return list;
        }

        public async Task<Result> Grants(SearchTopicGrantsDto grants)
        {
            if (grants.TopicIds.Length == 0) return Result.Successd();
            await Delete(grants.TopicIds);
            var list = grants.TopicIds.Select(tid => new SearchTopicGrantsEntity
            {
                Id = Guid.NewGuid(),
                RoleId = grants.RoleId,
                TopicId = tid,
                CreateTime = DateTimeOffset.Now
            });
            await this.BulkUpdate(list.ToList());
            return Result.Successd();
        }
    }
}
