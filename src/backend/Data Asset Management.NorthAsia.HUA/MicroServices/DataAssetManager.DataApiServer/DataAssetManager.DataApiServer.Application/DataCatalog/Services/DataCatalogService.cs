using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataCatalog.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.DatabaseAccessor;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using System.Text;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataCatalogService : BaseService<DataCatalogEntity, DataCatalogDto, string>, IDataCatalogService, ITransient
    {
        private readonly ILogger<DataCatalogService> _logger;
        public DataCatalogService(ISqlSugarClient db, IDistributedCacheService cache, ILogger<DataCatalogService> logger) : base(db, cache, false, true,true)
        {
            _logger = logger;
        }

        public async Task<List<DataCatalogEntity>> All()
        {
            //查询表的所有
            var list = await CurrentDb.Queryable<DataCatalogEntity>().OrderByDescending(f=>  f.CreateTime).ToListAsync();
            return list;
        }
        public async Task<List<DataCatalogEntity>> AllFromCache(bool clearCache = false)
        {
            //if (clearCache) await _cache.RemoveAsync(DataAssetManagerConst.DataCatalog_ListKey);
            return await _cache.GetObjectAsync(DataAssetManagerConst.DataCatalog_ListKey, async () => await All(), null, clearCache);
        }

        public async Task<List<DataCatalogEntity>> InitRedisHash()
        {
            var list = await AllFromCache();
            foreach (var item in list)
            {
                _cache.HashSet(DataAssetManagerConst.DataCatalog_HashKey, item.Id, item);
            }
            return list;
        }

        public override ISugarQueryable<DataCatalogEntity> BuildFilterQuery(DataCatalogDto filter)
        {
            filter.Name = filter.Name?.Trim()?.ToLower();
            filter.Code = filter.Code?.Trim()?.ToLower();
            return CurrentDb.Queryable<DataCatalogEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Name), f => SqlFunc.ToLower(f.Name).Contains(filter.Name))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Code), f => SqlFunc.ToLower(f.Code).Contains(filter.Code))
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .OrderBy(f => f.Sort);
                //.OrderByDescending(f => f.CreateTime);
        }

        public async Task<List<DataCatalogEntity>> GetTopic(string[] ctlIds, string searchkey = "")
        {
            searchkey = searchkey?.Trim()?.ToLower();
            var query = CurrentDb.Queryable<DataCatalogEntity>()
                 .Where(f => f.Status == 1)
                 .WhereIF(ctlIds != null & ctlIds.Count() > 0, f => ctlIds.Contains(f.Id))
                 .WhereIF(!string.IsNullOrWhiteSpace(searchkey), f =>
                   SqlFunc.ToLower(f.Name).Contains(searchkey) || SqlFunc.ToLower(f.Code).Contains(searchkey) || SqlFunc.ToLower(f.Remark).Contains(searchkey))
                 .OrderBy(f => f.Sort);
                 //.OrderByDescending(f => f.CreateTime);

            return await query.ToChildListAsync(t => t.ParentCtlId, ctlIds);
        }

        public async Task<List<TreeEntity>> GetTreeTopic(string[] ctlIds, string searchkey = "")
        {
            var ctlidList = new List<object>();
            searchkey = searchkey?.Trim()?.ToLower();
            var cacheKey = $"{DataAssetManagerConst.RedisKey}:TreeTopic";
            if ((ctlIds == null || ctlIds.Length == 0) && string.IsNullOrWhiteSpace(searchkey))
                cacheKey += "ALL";
            else if (!(ctlIds == null || ctlIds.Length == 0))
                cacheKey += string.Join("-", ctlIds);
            else if (!string.IsNullOrWhiteSpace(searchkey))
                cacheKey += searchkey;
            var getData = async () =>
            {
                var query = CurrentDb.Queryable<DataCatalogEntity>().Where(f=>f.Status==1).OrderBy(f => f.Sort).Select(f => new TreeEntity
                {
                    Key = f.Id,
                    PId = f.ParentCtlId,
                    Label = f.Name,
                    Value = f.Name,
                    Code = f.Code
                });

                //if ((ctlIds != null && ctlIds.Count() > 0) || !searchkey.IsNullOrWhiteSpace())
                //{
                ctlidList.AddRange(CurrentDb.Queryable<DataCatalogEntity>()
                     .Where(f => f.Status != 0)
                     .WhereIF(ctlIds != null && ctlIds.Count() > 0, f => ctlIds.Contains(f.Id))
                     .WhereIF(!string.IsNullOrWhiteSpace(searchkey), f => SqlFunc.ToLower(f.Name).Contains(searchkey) || SqlFunc.ToLower(f.Code).Contains(searchkey) || SqlFunc.ToLower(f.Remark).Contains(searchkey))
                     .Select(f => f.Id).ToList());

                return await query.ToTreeAsync(t => t.Children, t => t.PId, "", ctlidList.ToArray());
            };
            if (cacheKey.Length > 64) return await getData();
            else return await _cache.GetObjectAsync(cacheKey, getData, TimeSpan.FromSeconds(60));

            //}
            //else
            //{
            //    return await query.ToTreeAsync(t => t.Children, t => t.PId, "");
            //}
        }

        /// <summary>
        /// 获取顶级Topic
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataCatalogEntity>> GetTopTopic()
        {
            var allList = await AllFromCache();
            allList = allList.Where(f => f.Status != 0).ToList();
            var list = allList.Where(f => f.Status != 0)
                   .Where(f => f.ParentCtlId==null||f.ParentCtlId=="")
                   .ToList();
            return list;
        }


        /// <summary>
        /// 获取子级
        /// </summary>
        /// <param name="ctlIds"></param>
        /// <param name="searchkey"></param>
        /// <returns></returns>
        public async Task<List<DataCatalogEntity>> GetChildrensTopic(string[] ctlIds, string searchkey = "")
        {
            var allList = await AllFromCache();
            searchkey = searchkey?.Trim()?.ToLower();
            allList = allList.Where(f => f.Status != 0).ToList();
            var list = allList.Where(f => f.Status != 0)
                   .WhereIF(ctlIds != null & ctlIds.Count() > 0, f => ctlIds.Contains(f.ParentCtlId))
                   .WhereIF(!string.IsNullOrWhiteSpace(searchkey), f => SqlFunc.ToLower(f.Name).Contains(searchkey) || SqlFunc.ToLower(f.Code).Contains(searchkey) || SqlFunc.ToLower(f.Remark).Contains(searchkey))
                   .ToList();
            if (list.Count > 0)
                list.AddRange(await GetChildrensTopic(list.Select(f => f.Id), allList));
            return list;
        }

        /// <summary>
        /// 获取子级
        /// </summary>
        /// <param name="ctlIds"></param>
        /// <param name="listAll"></param>
        /// <returns></returns>
        private async Task<List<DataCatalogEntity>> GetChildrensTopic(IEnumerable<string> ctlIds, List<DataCatalogEntity> listAll)
        {
            var list = listAll.Where(f => ctlIds.Contains(f.ParentCtlId)).Distinct().ToList();
            if (list.Count() > 0)
                list.AddRange(await GetChildrensTopic(list.Select(f => f.Id), listAll));
            return list;
        }

        /// <summary>
        /// 获取父级
        /// </summary>
        /// <param name="ctlIds"></param>
        /// <returns></returns>
        public async Task<List<DataCatalogEntity>> GetParentTopic(string[] ctlIds)
        {
            if (ctlIds == null || ctlIds.Length == 0) return new List<DataCatalogEntity>();
            var list = await AllFromCache();
            list = list.Where(f => f.Status != 0).Where(f => ctlIds.Contains(f.Id)).ToList();
            if (list.Count > 0) list.AddRange(await GetParentTopic(list.Where(f => !f.ParentCtlId.IsNullOrWhiteSpace()).Select(f => f.ParentCtlId).Distinct().ToArray()));
            return list;
        }


        public async Task<List<TreeEntity>> GetTreeTopicFromCache()
        {
            var ctlidList = new List<object>();
            var list = (await AllFromCache()).Where(f => f.Status != 0).OrderBy(f => f.Sort)
                 .Select(f => new TreeEntity
                 {
                     Key = f.Id,
                     PId = f.ParentCtlId,
                     Label = f.Name,
                     Value = f.Name,
                     Code = f.Code
                 }).ToList();

            return UtilMethods.BuildTree(CurrentDb, list, "Key", "PId", "Children", "").ToList();
        }

        public async Task<PageResult<DataCatalogInfo>> QueryPage(DataCatalogDto filter)
        {
            filter.Name = filter.Name?.Trim()?.ToLower();
            filter.Code = filter.Code?.Trim()?.ToLower();
            var query = CurrentDb.Queryable<DataCatalogEntity>()
                .LeftJoin(CurrentDb.Queryable<DataCatalogEntity>(), (t, t1) => t.ParentCtlId == t1.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), (t, t1) => t.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Name), (t, t1) => SqlFunc.ToLower( t.Name).Contains(filter.Name))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Code), (t, t1) => SqlFunc.ToLower(t.Code).Contains( filter.Code))
                .WhereIF(filter.Status.HasValue, (t, t1) => t.Status == filter.Status)
                .OrderBy((t,t1)=>t.Sort)
                .Select((t, t1) => new DataCatalogInfo
                {
                    Id = t.Id,
                    Name = t.Name,
                    Code = t.Code,
                    ParentCtlId = t.ParentCtlId,
                    ParentName= t1.Name,
                    Remark = t.Remark,
                    Sort = t.Sort,
                    Status = t.Status,
                    CreateTime = t.CreateTime,
                    CreateBy = t.CreateBy,
                    UpdateTime = t.UpdateTime,
                    UpdateBy = t.UpdateBy
                });

            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            return new PageResult<DataCatalogInfo>(query.Count(), list, filter.PageNum, filter.PageSize);
        }

        public override async Task<int> Modify<TEntity>(TEntity entity, bool clearCache = true)
        {
            //检查不能引用自己和自己的父级中存在自己的父级
            if (entity is DataCatalogEntity)
            {
                var data = entity as DataCatalogEntity;
                if (data.Id == data.ParentCtlId) throw new AppFriendlyException("The Parent Subject cannot be itself", 6001);
                var pctlIds = await GetParentTopic(new string[] { data.ParentCtlId });
                if (pctlIds.Any(f => f.Id == data.Id))
                    throw new AppFriendlyException("The Parent Subject cannot be itself", 6001);
            }
            return await base.Modify(entity, clearCache);
        }

        public override async Task<bool> ModifyHasChange<TEntity>(TEntity entity, bool clearCache = true)
        {
            //检查不能引用自己和自己的父级中存在自己的父级
            if (entity is DataCatalogEntity)
            {
                var data = entity as DataCatalogEntity;
                if (data.Id == data.ParentCtlId) throw new AppFriendlyException("The Parent Subject cannot be itself", 6001);
                var pctlIds = await GetParentTopic(new string[] { data.ParentCtlId });
                if (pctlIds.Any(f => f.Id == data.Id))
                    throw new AppFriendlyException("The Parent Subject cannot be itself", 6001);
            }
            return await base.ModifyHasChange(entity, clearCache);
        }
    }
}
