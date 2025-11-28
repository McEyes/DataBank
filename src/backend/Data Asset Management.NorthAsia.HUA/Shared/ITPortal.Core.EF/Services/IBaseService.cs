

using SqlSugar;

using System.Linq.Expressions;

namespace ITPortal.Core.Services
{
    public interface IBaseService<T, Tdto, KeyType> where T : class,IEntity<KeyType>, new() where Tdto : class, IPageEntity<KeyType>, new()
    {
        ISqlSugarClient CurrentDb { get; }
        UserInfo CurrentUser { get; }

        Task<T> Get(KeyType id);
        Task<T> Single(Tdto filter);
        Task<List<T>> Query(Tdto filter);
        Task<PageResult<T>> PageQuery(Tdto filter);
        //Task<PageResult<T>> Page<TDto>(TDto filter) where TDto : class, IPageEntity<KeyType>, new();
        Task<PageResult<T>> Page(Tdto filter);//where TDto : class, IPageEntity<KeyType>, new();
        Task<int> Create<TEntity>(TEntity entity, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new();
        Task<bool> ModifyHasChange<TEntity>(TEntity entity, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new();
        Task<int> Modify<TEntity>(TEntity entity, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new();
        Task<bool> Delete(KeyType id, bool clearCache = true);
        Task<bool> Delete(dynamic[] ids, bool clearCache = true);

        ISugarQueryable<T> AsQueryable();

        ISugarQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class, IEntity<KeyType>, new();
        ISugarQueryable<TEntity> AsQueryable<TEntity,KeyType2>() where TEntity : class, IEntity<KeyType2>, new();
        ISugarQueryable<T> Paging<TDto>(ISugarQueryable<T> query, TDto filter) where TDto : class, IPageEntity<KeyType>, new();
        Task<int> BulkUpdate<T>(List<T> list, bool clearCache = true) where T : class, IEntity<KeyType>, new();
        Task<TEntity> Get<TEntity>(KeyType id) where TEntity : class, IEntity<KeyType>, new();
        Task<bool> Delete<TEntity>(KeyType id, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new();
        Task<bool> Delete<TEntity>(dynamic[] ids, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new();
        Task RefreshCache();
        Task<T> Get(Expression<Func<T, bool>> expression);
        Task<List<T>> Query(Expression<Func<T, bool>> expression);
        Task<bool> Delete(Expression<Func<T, bool>> expression);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression);
        List<T> GetList(Expression<Func<T, bool>> expression);
        Task<int> GetCount();
        Task<int> GetCount<TEntity>();
        string GetRedisKey<T>(string key, string keyType = "");
        string GetRedisKey(Type type, string key, string keyType = "");
        Task<int> GetCount<TEntity>(Expression<Func<TEntity, bool>> expression);
        //ISugarQueryable<T> Page(ISugarQueryable<T> query, T filter);
    }
}
