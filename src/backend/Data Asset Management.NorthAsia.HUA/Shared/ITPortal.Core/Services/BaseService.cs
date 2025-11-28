
using Elastic.Clients.Elasticsearch;

using Furion;
using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Extensions;

using Mapster;

using SqlSugar;

using StackExchange.Profiling.Internal;

using System.Linq.Expressions;

using static Grpc.Core.Metadata;

namespace ITPortal.Core.Services
{
    /// <summary>
    /// 非通过基类的增删改方法更新的数据，需要手动调用RefreshCache方法，否则，半小时内的缓存数据将是脏数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Tdto"></typeparam>
    /// <typeparam name="KeyType"></typeparam>
    public abstract class BaseService<T, Tdto, KeyType> : IBaseService<T, Tdto, KeyType> where T : class, IEntity<KeyType>, new() where Tdto : class, IPageEntity<KeyType>, new()
    {
        /// <summary>
        /// 当前用户信息
        /// </summary>
        public UserInfo CurrentUser { get; protected set; }

        public ISqlSugarClient CurrentDb { get; protected set; }
        protected IDistributedCacheService _cache { get; private set; }
        protected readonly IEventPublisher EventPublisher;
        protected bool NoRedis { get; set; } = false;
        protected bool NeedEventBus { get; set; } = false;
        protected bool EnableDiffLogEvent { get; set; } = false;
        public BaseService(ISqlSugarClient db, IDistributedCacheService cache, bool noRedis = false, bool needEventBus = false, bool enableDiffLogEvent=false)
        {
            CurrentDb = db;
            _cache = cache;
            CurrentUser = App.HttpContext.GetCurrUserInfo() ?? new UserInfo();
            EventPublisher = App.GetService<IEventPublisher>();
            NoRedis = noRedis;
            NeedEventBus = needEventBus;
            EnableDiffLogEvent= enableDiffLogEvent;
        }

        public virtual string GetRedisKey<T>(string key,string keyType = "")
        {
            return GetRedisKey(typeof(T), key, keyType);
        }

        public virtual string GetRedisKey(Type type, string key, string keyType = "")
        {
            return $"{DataAssetManagerConst.RedisKey}{type.Name}:{key}{(keyType.IsNullOrWhiteSpace() ? "" : ":" + keyType)}";
        }

        public abstract ISugarQueryable<T> BuildFilterQuery(Tdto filter);

        public virtual async Task<T> Get(KeyType id)
        {
            var fun = async () => { return await this.CurrentDb.Queryable<T>().InSingleAsync(id); };
            if (NoRedis) return await fun();
            else return await _cache.GetObjectAsync($"{DataAssetManagerConst.RedisKey}{typeof(T).Name}:{id}", fun, TimeSpan.FromSeconds(10));
        }
        public async Task<T> Get(Expression<Func<T, bool>> expression)
        {
            return await CurrentDb.Queryable<T>().Where(expression).FirstAsync();
        }

        public virtual async Task<TEntity> Get<TEntity>(KeyType id) where TEntity : class, IEntity<KeyType>, new()
        {
            var fun = async () => { return await this.CurrentDb.Queryable<TEntity>().InSingleAsync(id); };
            if (NoRedis) return await fun();
            return await _cache.GetObjectAsync($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:{id}", fun, TimeSpan.FromSeconds(10));
        }

        public virtual async Task<List<T>> Get(KeyType[] ids)
        {
            //return await _repository.GetListAsync(f => ids.Contains(f.Id));
            return await this.CurrentDb.Queryable<T>().Where(f => ids.Contains(f.Id)).ToListAsync();
        }
        public async Task<int> GetCount()
        {
            var fun = async () =>
            {
                return await CurrentDb.Queryable<T>().CountAsync();
            }; 
            if (NoRedis) return await fun();
            return await _cache.GetIntAsync($"{DataAssetManagerConst.RedisKey}{typeof(T).Name}:Count", fun, TimeSpan.FromSeconds(10)) ?? 0;
        }
        public async Task<int> GetCount<TEntity>()
        {
            var fun = async () =>
            {
                return await CurrentDb.Queryable<TEntity>().CountAsync();
            };
            if (NoRedis) return await fun();
            return await _cache.GetIntAsync($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:Count", fun, TimeSpan.FromSeconds(10)) ?? 0;
        }

        public async Task<int> GetCount<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            var fun = async () =>
            {
                return await CurrentDb.Queryable<TEntity>().Where(expression).CountAsync();
            };
            return await fun();
            //if (NoRedis) return await fun();
            //return await _cache.GetIntAsync($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:Count", fun, TimeSpan.FromSeconds(10)) ?? 0;
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression)
        {
            return await CurrentDb.Queryable<T>().Where(expression).ToListAsync();
        }

        public List<T> GetList(Expression<Func<T, bool>> expression)
        {
            return  CurrentDb.Queryable<T>().Where(expression).ToList();
        }

        public virtual async Task<T> Single(Tdto filter)
        {
            return await BuildFilterQuery(filter).FirstAsync();
        }

        public async Task<List<T>> Query(Tdto filter)
        {
            return await BuildFilterQuery(filter).ToListAsync();
        }

        public async Task<List<T>> Query(Expression<Func<T, bool>> expression)
        {
            return await CurrentDb.Queryable<T>().Where(expression).ToListAsync();
        }

        public virtual async Task<PageResult<T>> PageQuery(Tdto filter)
        {
            var query = BuildFilterQuery(filter);//.OrderBy(f => f.Id);
            return new PageResult<T>(query.Count(), await Paging(query, filter).ToListAsync(), filter.PageNum, filter.PageSize);
        }

        public virtual async Task<int> Create<TEntity>(TEntity entity, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new()
        {
            if (entity.Id == null || entity.Id.Equals("") || entity.Id.Equals(Guid.Empty))
            {
                if (typeof(KeyType) == typeof(string))
                {
                    entity.Id = SnowflakeIdGenerator.NextUid(19).Adapt<KeyType>();
                }
                else if (typeof(KeyType) == typeof(Guid))
                {
                    entity.Id = Guid.NewGuid().Adapt<KeyType>();
                }
            }
            if (entity is ICreateEntity<KeyType>)
            {
                var createEntity = (ICreateEntity<KeyType>)entity;
                createEntity.CreateBy = CurrentUser?.Name ?? "test";
                createEntity.CreateTime = DateTimeOffset.Now;
                if (entity is ICreateNameEntity)
                {
                    var createNameEntity = (ICreateNameEntity)entity;
                    createNameEntity.CreatedByName = CurrentUser?.Name ?? "test";
                }
            }
            if (entity is IAuditEntity<KeyType>)
            {
                var createEntity = (IAuditEntity<KeyType>)entity;
                createEntity.UpdateBy = CurrentUser?.Name ?? "test";
                createEntity.UpdateTime = DateTimeOffset.Now;
            }
            var result = await CurrentDb.Insertable(entity).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            if (!NoRedis)
            {
                _cache.SetObject($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:{entity.Id}", entity, TimeSpan.FromSeconds(10));
                if (clearCache) await RefreshCache<TEntity>();
            }
            if (result > 0)
            {
                await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Create", entity, typeof(TEntity));
            }
            return result;
        }

        public virtual async Task<int> Modify<TEntity>(TEntity entity, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new()
        {
            var model = await Get<TEntity>(entity.Id);
            if (model == null) throw new Exception("Data does not exist");
            if (entity is ICreateEntity<KeyType>)
            {
                var createEntity = entity as ICreateEntity<KeyType>;
                if (createEntity.CreateBy.IsNullOrWhiteSpace())
                    createEntity.CreateBy = ((ICreateEntity<KeyType>)model).CreateBy;
                if (createEntity.CreateTime <= new DateTime(1900, 1, 1))
                    createEntity.CreateTime = ((ICreateEntity<KeyType>)model).CreateTime;
                if (entity is ICreateNameEntity)
                {
                    var createNameEntity = (ICreateNameEntity)entity;
                    createNameEntity.CreatedByName = CurrentUser?.Name ?? "test";
                }
            }
            if (entity is IAuditEntity<KeyType>)
            {
                var createEntity = entity as IAuditEntity<KeyType>;
                createEntity.UpdateBy = CurrentUser?.Name ?? "test";
                createEntity.UpdateTime =DateTimeOffset.Now;
            }
            if (!NoRedis)
            {
                _cache.SetObject($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:{entity.Id}", entity, TimeSpan.FromSeconds(10));
                if (clearCache) await RefreshCache<TEntity>();
            }
            var result  = await CurrentDb.Updateable(entity).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandHasChangeAsync();
            if (result)
            {
                await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Modify", entity, typeof(TEntity));
                return 1;
            }
            return 0;
        }

        public virtual async Task<bool> ModifyHasChange<TEntity>(TEntity entity, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new()
        {
            var model = await Get<TEntity>(entity.Id);
            if (model == null) throw new Exception($"{typeof(TEntity).Name}.id:{entity.Id}数据不存在");
            if (entity is ICreateEntity<KeyType>)
            {
                var createEntity = entity as ICreateEntity<KeyType>;
                if (createEntity.CreateBy.IsNullOrWhiteSpace())
                    createEntity.CreateBy = ((ICreateEntity<KeyType>)model).CreateBy;
                if (createEntity.CreateTime <= new DateTime(1900, 1, 1))
                    createEntity.CreateTime = ((ICreateEntity<KeyType>)model).CreateTime;
                if (entity is ICreateNameEntity)
                {
                    var createNameEntity = (ICreateNameEntity)entity;
                    createNameEntity.CreatedByName = CurrentUser?.Name ?? "test";
                }
            }
            if (entity is IAuditEntity<KeyType>)
            {
                var createEntity = (IAuditEntity<KeyType>)entity;
                createEntity.UpdateBy = CurrentUser?.Name ?? "test";
                createEntity.UpdateTime = DateTimeOffset.Now;
            }
            if (!NoRedis)
            {
                _cache.SetObject($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:{entity.Id}", entity, TimeSpan.FromSeconds(10));
                if (clearCache) await RefreshCache<TEntity>();
            }
            var result = await CurrentDb.Updateable(entity).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandHasChangeAsync();
            if (result)
            {
                await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Modify", entity, typeof(TEntity));
            }
            return result;
        }

        public virtual async Task<bool> Delete(KeyType id, bool clearCache = true)
        {
            var result = await CurrentDb.Deleteable<T>().Where(it => it.Id.Equals(id)).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            if (!NoRedis)
            {
                await _cache.DelayRemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(T).Name}:{id}");
                if (clearCache) await RefreshCache<T>();
            }

            await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Delete", id, typeof(T));
            return result > 0;
        }

        public virtual async Task<bool> Delete<TEntity>(KeyType id, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new()
        {
            var result = await CurrentDb.Deleteable<TEntity>().Where(it => it.Id.Equals(id)).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            //var result = await _repository.DeleteByIdAsync(id);

            if (!NoRedis)
            {
                await _cache.DelayRemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(T).Name}:{id}");
                if (clearCache) await RefreshCache<TEntity>();
            }
            if (result > 0)
            {
               await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Delete", id, typeof(TEntity));
            }
            return result > 0;
        }

        public virtual async Task<bool> Delete(dynamic[] ids, bool clearCache = true)
        {
            var result = await CurrentDb.Deleteable<T>().In(ids).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            if (!NoRedis)
            {
                foreach (var id in ids)
                {
                    await _cache.DelayRemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(T).Name}:{id}");
                }
                if (clearCache) await RefreshCache<T>();
            }
            if (result > 0)
            {
                await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Deletes", ids, typeof(T));
            }
            return result > 0;
        }

        public async Task<bool> Delete(Expression<Func<T, bool>> expression)
        {
            var result = await CurrentDb.Deleteable<T>().Where(expression).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            return result > 0;
        }


        public virtual async Task<bool> Delete<TEntity>(dynamic[] ids, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new()
        {
            var result = await CurrentDb.Deleteable<TEntity>().In(ids).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            if (!NoRedis)
            {
                foreach (var id in ids)
                {
                    await _cache.DelayRemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(T).Name}:{id}");
                }
                if (clearCache) await RefreshCache<TEntity>();
            }
            if (result > 0)
            {
                await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Deletes", ids, typeof(TEntity));
            }
            return result > 0;
        }

        public virtual ISugarQueryable<T> AsQueryable()
        {
            return this.CurrentDb.Queryable<T>();
        }

        public virtual ISugarQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class, IEntity<KeyType>, new()
        {
            return this.CurrentDb.Queryable<TEntity>();
        }

        public virtual ISugarQueryable<TEntity> AsQueryable<TEntity, KeyType2>() where TEntity : class, IEntity<KeyType2>, new()
        {
            return this.CurrentDb.Queryable<TEntity>();
        }

        public virtual async Task<PageResult<T>> Page(Tdto filter)
        {
            var query = BuildFilterQuery(filter);
            return new PageResult<T>(query.Count(), await Paging(query, filter).ToListAsync(), filter.PageNum, filter.PageSize);
        }

        public virtual ISugarQueryable<T> Paging<TDto>(ISugarQueryable<T> query, TDto filter) where TDto : class, IPageEntity<KeyType>, new()
        {
            return query.Skip(filter.SkipCount).Take(filter.PageSize);
        }
        public virtual ISugarQueryable<TEntity> Page<TEntity,TDto>(ISugarQueryable<TEntity> query, TDto filter)
            where TDto : class, IPageEntity<KeyType>, new()
            where TEntity:IEntity<KeyType>, new()
        {
            return query.Skip(filter.SkipCount).Take(filter.PageSize);
        }

        public async Task<int> BulkUpdate<TEntity>(List<TEntity> list, bool clearCache = true) where TEntity : class, IEntity<KeyType>, new()
        {
            if (list == null || list.Count == 0) return 0;
            var result = await CurrentDb.Storageable(list).ExecuteSqlBulkCopyAsync();
            if (!NoRedis && clearCache) await RefreshCache<TEntity>();
            if (result > 0)
            {
                await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:BulkUpdate", list, typeof(TEntity));
            }
            return result;
        }

        /// <summary>
        /// 新增，修改，删除刷新缓存用
        /// </summary>
        public virtual async Task RefreshCache()
        {
            await RefreshCache<T>();
        }

        /// <summary>
        /// 新增，修改，删除刷新缓存用
        /// </summary>
        public virtual async Task RefreshCache<TEntity>()
        {
            if (NoRedis) return;
            var key = $"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}";
            await _cache.DelayRemoveAsync($"{key}:Hash");
            await _cache.DelayRemoveAsync($"{key}:List");
            await Task.CompletedTask;
        }

        public async Task SendLogEvent(string eventId, object payload, Type modelType, CancellationToken cancellationToken = default)
        {
            if (NeedEventBus)
            {
                await EventPublisher.PublishAsync(new LightElasticSearch.DBEventSource(eventId, modelType, payload));
            }
        }

        public async Task SendEvent(string eventId, object payload = default, CancellationToken cancellationToken = default)
        {
            if (NeedEventBus) await EventPublisher.PublishAsync(eventId, payload, cancellationToken);
        }
    }
}
