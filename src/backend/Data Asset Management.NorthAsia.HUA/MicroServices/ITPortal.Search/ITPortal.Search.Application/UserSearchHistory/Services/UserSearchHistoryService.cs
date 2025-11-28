using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Search.Application.UserSearchHistory.Dtos;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.UserSearchHistory.Services
{
    public class UserSearchHistoryService : BaseService<UserSearchHistoryEntity, UserSearchHistoryDto, Guid>, IUserSearchHistoryService,ITransient
    {
        public UserSearchHistoryService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache, true)
        {
        }

        public override ISugarQueryable<UserSearchHistoryEntity> BuildFilterQuery(UserSearchHistoryDto filter)
        {
            return CurrentDb.Queryable<UserSearchHistoryEntity>()
               .WhereIF(filter.Keyword.IsNotNullOrWhiteSpace(), f => f.Keyword == filter.Keyword)
               .OrderByDescending(f => f.CreationTime);
        }

        public async Task<List<UserSearchHistoryDto>> GetHistoryListAsync()
        {
            var list = await CurrentDb.Queryable<UserSearchHistoryEntity>()
                 .Where(p => p.UserId == CurrentUser.UserId)
                 .OrderByDescending(p => p.CreationTime)
                 .Take(10).ToListAsync();

            return list.Adapt<List<UserSearchHistoryDto>>();
        }

        public async  Task<bool> Delete(List<Guid> ids)
        {
            var result = await CurrentDb.Deleteable<UserSearchHistoryEntity>().In(ids).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            return result > 0;
        }

    }
}
