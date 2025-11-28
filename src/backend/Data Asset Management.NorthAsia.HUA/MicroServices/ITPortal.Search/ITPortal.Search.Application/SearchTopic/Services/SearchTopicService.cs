using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopic.Dtos;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.SearchTopic.Services
{
    public class SearchTopicService : BaseService<SearchTopicEntity, SearchTopicDto, Guid>, ISearchTopicService, ITransient
    {

        public SearchTopicService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache,true)
        {
        }

        public override ISugarQueryable<SearchTopicEntity> BuildFilterQuery(SearchTopicDto filter)
        {
            return CurrentDb.Queryable<SearchTopicEntity>()
               .WhereIF(!string.IsNullOrWhiteSpace(filter.Keyword), f => f.Topic == filter.Keyword || f.Name == filter.Keyword)
               .WhereIF(!string.IsNullOrWhiteSpace(filter.Topic), f => f.Topic == filter.Topic)
               .WhereIF(!string.IsNullOrWhiteSpace(filter.Name), f => f.Name == filter.Name)
               .WhereIF(filter.IsEnable.HasValue, f => f.IsEnable == filter.IsEnable)
               .WhereIF(filter.IsPublic.HasValue, f => f.IsPublic == filter.IsPublic)
               .OrderByDescending(f => f.CreateTime);
        }



        public async Task<SearchTopicEntity> GetByNameOrTopic(string name, string topic)
        {
            var fun = async () =>
            {
                var entity = await AsQueryable().FirstAsync(p => p.Name == name || p.Topic == topic);
                return entity;
            };
            return await _cache.GetObjectAsync<SearchTopicEntity>($"{DataAssetManagerConst.RedisKey}SearchTopicEntity:{name}_{topic}", fun);
        }

        public async Task<SearchTopicEntity> GetByTopic(string topic)
        {
            var fun = async () =>
            {
                var entity = await AsQueryable().FirstAsync(p => p.Topic.Equals(topic));
                return entity;
            };
            return await _cache.GetObjectAsync<SearchTopicEntity>($"{DataAssetManagerConst.RedisKey}SearchTopicEntity:topic:{topic}", fun);
        }

        public async Task<List<SearchTopicEntity>> GetByTopics(List<string> topics)
        {
            if (topics.Count == 0) return new List<SearchTopicEntity>();

            var list = await AsQueryable()
                .Where(p => topics.Contains(p.Topic))
                .ToListAsync();

            return list;
        }

        public async Task<(IEnumerable<SearchTopicEntity> items, long totalCount)> GetByTopicIds(
            List<Guid> topicIds,
            int pageIndex = -1,
            int pageSize = -1)
        {
            if (topicIds.Count == 0) return (new List<SearchTopicEntity>(), 0);

            var queryable = AsQueryable()
                .Where(p => topicIds.Contains(p.Id));

            long totalCount = 0;
            List<SearchTopicEntity> list = new List<SearchTopicEntity>();
            if (pageIndex == -1 || pageSize == -1)
            {
                list = await queryable.ToListAsync();
                totalCount = list.Count;
            }
            else
            {
                totalCount = await queryable.CountAsync();
                list = await queryable
                    .OrderByDescending(p => p.CreateTime)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return (list, totalCount);
        }


        public async Task<PageResult<SearchTopicDto>> GetListByPage(PageEntity<Guid> filter)
        {
            string keywordLower = string.Empty;
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                keywordLower = filter.Keyword.ToLower().Trim();
            }
            var query = AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(keywordLower), p =>
                    p.Name == keywordLower || p.Topic == keywordLower);

            return new PageResult<SearchTopicDto>(query.Count(), (await Paging(query, filter).ToListAsync()).Adapt<List<SearchTopicDto>>(), filter.PageNum, filter.PageSize);
        }

    }
}
