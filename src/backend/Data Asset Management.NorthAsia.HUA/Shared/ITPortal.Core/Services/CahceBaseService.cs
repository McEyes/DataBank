
using ITPortal.Core.DistributedCache;

using SqlSugar;

namespace ITPortal.Core.Services
{
    public abstract class CahceBaseService<T, Tdto, KeyType> : BaseService<T, Tdto, KeyType>, ICahceQueryService<T, Tdto, KeyType> where T : class, IEntity<KeyType>, new() where Tdto : class, IPageEntity<KeyType>, new()
    {
        public CahceBaseService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache)
        {
        }

        public async Task<T> GetByCache(KeyType id)
        {
            return await _cache.GetObjectAsync(id + "", async () =>
            {
                return await Get(id);
            });
        }
    }
}
