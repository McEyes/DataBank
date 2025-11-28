using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataAuthorizeUser.Dtos;
using DataAssetManager.DataApiServer.Application.DataCatalog.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch;

using Furion.DistributedIDGenerator;
using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using System.Linq;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataTableService : BaseService<DataTableEntity, DataTableInfo, string>, IDataTableService, ITransient
    {
        private readonly ILogger<DataTableService> _logger;
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IDataColumnService _columnService;
        private readonly IDataUserService _dataUserService;
        private readonly IDistributedIDGenerator _idGenerator;
        private readonly IEventPublisher _eventPublisher;

        public DataTableService(ISqlSugarClient db, IDistributedCacheService cache, ILogger<DataTableService> logger,
             IDataColumnService columnService, IDataUserService dataUserService, 
             IDistributedIDGenerator idGenerator, IDataCatalogService dataCatalogService,
            IEventPublisher eventPublisher) : base(db, cache, false, true, true)
        {
            _logger = logger;
            _dataCatalogService = dataCatalogService;
            _columnService = columnService;
            _dataUserService = dataUserService;
            _idGenerator = idGenerator;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// 获取所有表的修改真实表数据
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, DataTableRename>> GetAllTableRename(bool clearCache = false)
        {
            var fun = async () =>
            {
                var list = await CurrentDb.Queryable<DataTableRename>().Where(f => f.Effect == true && f.TableName != "" && f.NewTableName != "").ToListAsync();
                var dict = new Dictionary<string, DataTableRename>();
                foreach (var item in list)
                    dict.TryAdd(item.TabkeKey, item);
                return dict;
            };
            return await _cache.GetObjectAsync(GetRedisKey<DataTableRename>("alias"), fun, TimeSpan.FromMinutes(10), clearCache);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="tableName"></param>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public async Task<DataTableRename> GetTableRename(string sourceName, string tableName, bool clearCache = false)
        {
            var aliasDict = await GetAllTableRename(clearCache);
            var key = $"{sourceName}-{tableName}".ToLower();
            if (aliasDict.TryGetValue(key, out DataTableRename rename))
                return rename;
            return null;
        }


        /// <summary>
        /// 获取所有表的别名数据
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, HashSet<string>>> GetAllTableAlias(bool clearCache = false)
        {
            var fun = async () =>
            {
                var list = await CurrentDb.Queryable<DataTableAlias>().Where(f => f.TableName != "" && f.TableAlias != "").ToListAsync();
                return list.GroupBy(f => f.TableName)
                  .Select(f =>
                          new KeyValuePair<string, HashSet<string>>(f.Key.ToLower(), 
                          f.Select(f => f.TableAlias.ToLower()).ToHashSet()))
                  .ToDictionary();
            };
            return await _cache.GetObjectAsync(GetRedisKey<DataTableAlias>("alias"), fun, TimeSpan.FromMinutes(10), clearCache);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public async Task<HashSet<string>> GetTableAlias(string tableName, bool clearCache = false)
        {
            var aliasDict = await GetAllTableAlias(clearCache);
            if (aliasDict.TryGetValue(tableName.ToLower(), out HashSet<string> aliasHash))
                return aliasHash;
            return new HashSet<string>();
        }

        /// <summary>
        /// 检查是否表别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableAlias"></param>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public async Task<bool> CheckIsTableAlias(string tableName, string tableAlias, bool clearCache = false)
        {
            var aliasDict = await GetAllTableAlias(clearCache);
            return aliasDict.TryGetValue(tableName.ToLower(), out HashSet<string> aliasHash) && aliasHash.Contains(tableAlias.ToLower());
        }

        public async Task<List<string>> GetTableTags()
        {
            return await CurrentDb.Queryable<MetaDataExtEntity>()
                   .Where(f => f.Tag != "" && f.Tag != null)
                   .Select(f => f.Tag).Distinct().ToListAsync();
        }

        /// <summary>
        /// 获取每个表的api缓存信息
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, int>> GetTableVisited(bool clearCache = false)
        {
            var fun = async () =>
            {

                var list = await CurrentDb.Queryable<DataTableVisited30Entity>().ToListAsync();
                var keyValuePairs = new Dictionary<string, int>();
                foreach (var item in list)
                {
                    keyValuePairs.Add(item.TableId, item.VisitedTimes);
                }
                return keyValuePairs;
            };
            return await _cache.GetObjectAsync(GetRedisKey<DataTableVisited30Entity>("TableVisited"), fun, TimeSpan.FromMinutes(1), clearCache);
        }


        public async Task<List<DataTableInfo>> All()
        {
            var list= await CurrentDb.Queryable<DataTableEntity>()
                    .LeftJoin<DataCatalogTableMapping>((dt, ct) => dt.Id == ct.TableId)//多个条件用&&
                    .LeftJoin<DataCatalogEntity>((dt, ct, dc) => ct.CatalogId == dc.Id)//多个条件用&&
                    .LeftJoin<DataSourceEntity>((dt, ct, dc, ds) => dt.SourceId == ds.Id)
                    .LeftJoin<DataSecurityLevelEntity>((dt, ct, dc, ds, dsl)
                            => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                    .Select((dt, ct, dc, ds, dsl) =>
                     new DataTableEntity
                     {
                         Id = dt.Id,
                         Alias = dt.Alias,
                         TableName = dt.TableName,
                         TableComment = dt.TableComment,
                         Reviewable = dt.Reviewable,
                         JsonSqlConfig = dt.JsonSqlConfig,
                         Status = dt.Status,
                         UpdateFrequency = dt.UpdateFrequency,
                         UpdateMethod = dt.UpdateMethod,
                         DataTimeRange = dt.DataTimeRange,
                         Remark = dt.Remark,
                         CtlId = dc.Id,
                         CtlCode = dc.Code,
                         CtlName = dc.Name,
                         CtlRemark = dc.Remark,
                         SourceId = ds.Id,
                         SourceName = ds.SourceName,
                         LevelId = dsl.LevelId,
                         LevelName = dsl.LevelName,
                         OwnerDepart= dt.OwnerDepart,
                         QualityScore = dt.QualityScore,
                         LastScore = dt.LastScore,
                         DataCategory = dt.DataCategory,
                         UpdateCategory = dt.UpdateCategory,
                         //OwnerId = dt.OwnerId,
                         //OwnerName = dt.OwnerName,
                         CreateBy = dt.CreateBy,
                         CreateTime = dt.CreateTime,
                     }).Distinct()
                    .ToListAsync();
            return list.Adapt<List<DataTableInfo>>();
        }


        public async Task<List<DataTableInfo>> AllFromCache(bool clearCache = false)
        {
            //if (clearCache) await _cache.RemoveAsync(DataAssetManagerConst.DataTable_ListKey);
            return await _cache.GetObjectAsync(DataAssetManagerConst.DataTable_ListKey, All,null,clearCache);
        }

        public async Task<List<DataTableInfo>> InitRedisHash(bool clearCache = false)
        {
            var list = await AllFromCache(clearCache);
            foreach (var item in list)
            {
                _cache.HashSet(DataAssetManagerConst.DataTable_HashKey, item.Id, item);
            }
            return list;
        }


        public async Task<DataTableInfo> GetTableInfoByName(string tableName)
        {
            var list = await AllFromCache();
            return list.FirstOrDefault(x => x.TableName == tableName);
        }

        public async Task<List<DataColumnEntity>> GetTableCloumnsByName(string tableName)
        {
            var list = await AllFromCache();
            var tableInfo = list.FirstOrDefault(x => x.TableName.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
            if (tableInfo != null)
                return await _columnService.GetByTableId(tableInfo.Id);
            return null;
        }


        public async Task<List<DataColumnEntity>> GetTableCloumnsByTableId(string tableId)
        {
            return await _columnService.GetByTableId(tableId);
        }

        public async Task<List<DataTableAuthorizeUser>> TableUserAll()
        {
            try
            {
                var tableQuery = CurrentDb.Queryable<DataTableEntity>().Where(f => f.Status != 0);
                //.InnerJoin<DataSecurityLevelEntity>((dt, dsl) =>
                //dt.SourceId == dsl.ObjectId && dsl.ObjectType == "table" && dsl.LevelId != "2" && dsl.LevelId != "1");
                //.Select((dt, dsl) =>
                //new { dt.Id, dt.SourceId, dt.Status, dt.TableName, dt.Alias, dsl.LevelId });

                var userQuery = tableQuery
                     .InnerJoin<DataAuthorizeUserEntity>((dt, dau) => dt.Id == dau.ObjectId)
                    .Select((dt, dau) =>
                     new DataTableAuthorizeUser()
                     {
                         Id = dt.Id,
                         TableName = dt.TableName,
                         //SourceId = td.SourceId,
                         //Status = td.Status,
                         //Alias = td.Alias,
                         //LevelId = td.LevelId,
                         UserId = dau.UserId
                     })
                    .Distinct();

                var ownerQuery = tableQuery
                    .InnerJoin<DataAuthorizeOwnerEntity>((dt, dau) => dt.Id == dau.ObjectId)
                    .Select((dt, dau) =>
                    new DataTableAuthorizeUser()
                    {
                        Id = dt.Id,
                        TableName = dt.TableName,
                        //SourceId = td.SourceId,
                        //Status = td.Status,
                        //Alias = td.Alias,
                        //LevelId = td.LevelId,
                        UserId = dau.OwnerId
                    })
                    .Distinct();

                return await CurrentDb.UnionAll(userQuery, ownerQuery).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"获取tableUsers异常:{ex.Message},\r\n{ex.StackTrace}");
            }
            return new List<DataTableAuthorizeUser>();
        }


        public async Task<List<List<string>>> InitTableUserFromCache(bool clearCache = false)
        {
           var fun= async () =>
            {
                var list = await TableUserAll();
                IDictionary<string, List<string>> tableUsers = new Dictionary<string, List<string>>();
                foreach (var item in list)
                {
                    if (!tableUsers.ContainsKey(item.Id))
                        tableUsers.Add(item.Id, new List<string>());
                    tableUsers[item.Id].Add(item.UserId);
                }
                foreach (var item in tableUsers)
                {
                    _cache.HashSet(DataAssetManagerConst.DataTable_UserHashKey, item.Key, item.Value);
                }
                return tableUsers.Values.ToList();
            };
            if (clearCache)
            {
                var data = await fun();
                return data;
            }
            return await _cache.HashGetAllAsync(DataAssetManagerConst.DataTable_UserHashKey, fun);
        }

        public List<string> GetTableAuthorizeUsers(string tableId)
        {
            return _cache.HashGet<List<string>>(DataAssetManagerConst.DataTable_UserHashKey, tableId);
        }

        /// <summary>
        /// 检查数据库是否有权限
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool CheckUserHasTable(string tableId, string userid)
        {
            var list = _cache.HashGet<List<string>>(DataAssetManagerConst.DataTable_UserHashKey, tableId);
            if (list?.Count == 0)
            {//击穿校验,主要用于换成刷新瞬间和缓存击穿再次数据库级别校验
                return CheckSecretTable(tableId, userid);
            }
            return list?.Any(f => f == userid) ?? false;
        }

        /// <summary>
        /// 检查是否有权限
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool CheckSecretTable(string tableId, string userid)
        {
            return CurrentDb.Queryable<AssetClientSecretsEntity>().Where(s => SqlFunc.ToLower(s.Secrets) == userid.ToLower())
                  .InnerJoin(CurrentDb.Queryable<AssetClientScopesEntity>().Where(c => SqlFunc.ToLower(c.ObjectId) == tableId.ToLower()),
                  (s, c) => s.ClientId == c.ClientId)
                  .Any();
        }


        public override ISugarQueryable<DataTableEntity> BuildFilterQuery(DataTableInfo filter)
        {
            var query = CurrentDb.Queryable<DataTableEntity>()
                   .LeftJoin(CurrentDb.Queryable<DataAuthorizeOwnerEntity>(), (f, o) => f.Id == o.ObjectId)
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), (f, o) => f.Id == filter.Id)
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.SourceId), (f, o) => f.SourceId == filter.SourceId)
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.OwnerId), (f, o) => o.OwnerId == filter.OwnerId)
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.TableName), (f, o) => SqlFunc.ToLower(f.TableName) == filter.TableName.ToLower())
                  .WhereIF(filter.Status.HasValue, (f, o) => f.Status == filter.Status)
                  .OrderByDescending((f, o) => f.CreateTime)
                  .Select(f => f);
            //if (!CurrentUser.Roles.Any(f => f.Contains("admin")))
            //{//非管理员只能查看自己的数据
            //    var roles = CurrentUser.Roles;
            //    var authorQuery=  CurrentDb.Queryable<DataAuthorizeEntity>().Where(f=> roles.Contains(f.RoleId));

            //}
            return query;
        }

        public override async Task<List<DataTableEntity>> Get(string[] ids)
        {
            return await this.CurrentDb.Queryable<DataTableEntity>().Where(f => ids.Contains(f.Id)).OrderBy(f => f.TableName).ToListAsync();
        }

        public async Task<PageResult<DataTableInfo>> GetTopicTable(TopicTableQuery filter)
        {
            List<string> ctlids = new List<string>();
            if (filter.CtlId.IsNotNullOrWhiteSpace())
            {
                ctlids.Add(filter.CtlId);
                //ctlids.AddRange(filter.CtlId.Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries));
                var childrens = await _dataCatalogService.GetChildrensTopic(ctlids.ToArray(), filter.Keyword);
                ctlids.AddRange(childrens.Select(f => f.Id));
            }
            if (filter.Keyword.IsNotNullOrWhiteSpace())
            {
                filter.Keyword = filter.Keyword.Trim().ToLower();
            }
            ctlids = ctlids.Distinct().ToList();
            var query = CurrentDb.Queryable<DataTableEntity>()
                           .InnerJoin<DataCatalogTableMapping>((dt, ct) => dt.Id == ct.TableId)
                           .InnerJoin<DataCatalogEntity>((dt, ct, dc) => ct.CatalogId == dc.Id && dc.Status == 1)
                           .InnerJoin<DataSourceEntity>((dt, ct, dc, ds) => dt.SourceId == ds.Id)
                           .LeftJoin<DataSecurityLevelEntity>((dt, ct, dc, ds, dsl)
                                   => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                           .LeftJoin<DataAuthorizeOwnerEntity>((dt, ct, dc, ds, dsl, dao) => dao.ObjectId == dt.Id)
                           .LeftJoin<MetaDataExtEntity>((dt, ct, dc, ds, dsl, dao, dee) => dee.Id == dt.Id)
                           .LeftJoin<DataColumnEntity>((dt, ct, dc, ds, dsl, dao, dee, dcol) => dcol.TableId == dt.Id)
                           .WhereIF(filter.Id.IsNotNullOrWhiteSpace(), (dt, ct, dc, ds, dsl, dao, dee, dcol) => dt.Id.Equals(filter.Id))
                           .WhereIF(ctlids.Count > 0, (dt, ct, dc, ds, dsl, dao, dee, dcol) => ctlids.Contains(dc.Id))
                           //.WhereIF(filter.OrderField.IsNotNullOrWhiteSpace()&& filter.OrderField.Contains("DESC"), (dt, ct, dc, ds, dsl, dao, dee, dcol) => dt.QualityScore!=null)
                           .WhereIF(filter.DataCategory.IsNotNullOrWhiteSpace(), (dt, ct, dc, ds, dsl, dao, dee) => dt.DataCategory == filter.DataCategory)
                           .WhereIF(!filter.Keyword.IsNullOrWhiteSpace(), (dt, ct, dc, ds, dsl, dao, dee, dcol) =>
                                 SqlFunc.ToLower(dt.TableComment).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.TableName).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.Alias).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dee.Tag).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dcol.ColName).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dcol.ColComment).Contains(filter.Keyword))
                           .GroupBy((dt, ct, dc, ds, dsl, dao, dee) => new
                           {
                               ds.SourceName,
                               dt.SourceId,
                               dt.Id,
                               dt.TableName,
                               dt.Alias,
                               dt.Reviewable,
                               dt.TableComment,
                               dt.Status,
                               dt.UpdateFrequency,
                               dt.UpdateMethod,
                               dt.DataTimeRange,
                               dt.CreateBy,
                               dt.CreateTime,
                               CtlId = dc.Id,
                               CtlCode = dc.Code,
                               CtlName = dc.Name,
                               CtlRemark = dc.Remark,
                               dee.Tag,
                               dee.NeedSup,
                               //dee.NeedSup,
                               dsl.LevelId,
                               dsl.LevelName,
                               dt.QualityScore,
                               dt.DataCategory,
                               dt.LastScore,
                               dt.UpdateCategory,
                               dt.OwnerDepart
                           })
                           .Select((dt, ct, dc, ds, dsl, dao, dee) =>
                            new DataTableInfo
                            {
                                Id = dt.Id,
                                SourceId = dt.SourceId,
                                SourceName = ds.SourceName,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                LevelId = dsl.LevelId,
                                LevelName = dsl.LevelName,
                                //OwnerId = SqlFunc.MappingColumn(default(string), "GROUP_CONCAT(DISTINCT dao.owner_id,',')"),
                                //OwnerName = SqlFunc.MappingColumn(default(string), "GROUP_CONCAT(DISTINCT dao.owner_name,',')"),
                                OwnerId = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_id,',')"),
                                OwnerName = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_name,',')"),
                                OwnerDept =dt.OwnerDepart?? SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_dept,',')"),//STRING_AGG(DISTINCT CAST(owner_id AS TEXT), ',')
                                CtlId = dc.Id,
                                CtlCode = dc.Code,
                                CtlName = dc.Name,
                                CtlRemark = dc.Remark,
                                //Data = "",//url清单，逗号分割
                                Tag = dee.Tag,
                                NeedSup = dee.NeedSup,
                                UpdateFrequency = dt.UpdateFrequency,
                                UpdateMethod = dt.UpdateMethod,
                                DataTimeRange = dt.DataTimeRange,
                                QualityScore = dt.QualityScore,
                                LastScore=  dt.LastScore,
                                DataCategory=dt.DataCategory,
                                UpdateCategory = dt.UpdateCategory,
                                CreateBy = dt.CreateBy,
                                CreateTime = dt.CreateTime,
                            });
            if (filter.OrderField.IsNotNullOrWhiteSpace())
            {
                if (filter.OrderField.Contains("QualityScore", StringComparison.CurrentCultureIgnoreCase)) filter.OrderField = " CASE WHEN dt.quality_score IS NULL THEN 1 ELSE 0 END, " + filter.OrderField;
                query = query.OrderBy(filter.OrderField);
            }
            //query = query.OrderByIF(filter.OrderField.IsNotNullOrWhiteSpace(), " CASE WHEN QualityScore IS NULL THEN 1 ELSE 0 END, " + filter.OrderField);
            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            var apiList = await _cache.HashGetAllAsync<RouteInfo>(DataAssetManagerConst.RouteRedisKey);
            var tableVisited = await GetTableVisited();
            var hostUrl = DataAssetManagerConst.HostUrl.Replace("http://", "https://");
            list.ForEach(item =>
            {
                item.OwnerId = item.OwnerId?.TrimEnd(',');
                item.OwnerName = item.OwnerName?.TrimEnd(',');
                if(item.OwnerDept.IsNullOrWhiteSpace()) 
                item.OwnerDept = item.OwnerDept?.TrimEnd(',');
                item.Data = string.Join(",", apiList.Where(f => f.ExecuteConfig?.tableId == item.Id)
                    .Select(f =>$"[{f.ReqMethod.ToUpper()}]{hostUrl}{f.ApiServiceUrl}").ToArray());
                if (tableVisited.TryGetValue(item.Id, out int visited))
                    item.VisitedTimes = visited;
            });
            return new PageResult<DataTableInfo>(query.Count(), list,
                filter.PageNum, filter.PageSize);
        }


        public async Task<PageResult<DataTableInfo>> GetCurentTopicTable(TopicTableQuery filter)
        {
            List<string> ctlids = new List<string>();
            if (!filter.CtlId.IsNullOrWhiteSpace())
            {
                ctlids.Add(filter.CtlId);
            }
            if (filter.Keyword.IsNotNullOrWhiteSpace())
            {
                filter.Keyword = filter.Keyword.Trim().ToLower();
            }
            ctlids = ctlids.Distinct().ToList();
            var query = CurrentDb.Queryable<DataTableEntity>()
                           .InnerJoin<DataCatalogTableMapping>((dt, ct) => dt.Id == ct.TableId)
                           .InnerJoin<DataCatalogEntity>((dt, ct, dc) => ct.CatalogId == dc.Id && dc.Status == 1)
                           .InnerJoin<DataSourceEntity>((dt, ct, dc, ds) => dt.SourceId == ds.Id)
                           .LeftJoin<DataSecurityLevelEntity>((dt, ct, dc, ds, dsl)
                                   => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                           .LeftJoin<DataAuthorizeOwnerEntity>((dt, ct, dc, ds, dsl, dao) => dao.ObjectId == dt.Id)
                           .LeftJoin<MetaDataExtEntity>((dt, ct, dc, ds, dsl, dao, dee) => dee.Id == dt.Id)
                           .LeftJoin<DataColumnEntity>((dt, ct, dc, ds, dsl, dao, dee, dcol) => dcol.TableId == dt.Id)
                           .WhereIF(ctlids.Count > 0, (dt, ct, dc, ds, dsl, dao, dee, dcol) => ctlids.Contains(dc.Id))
                           .WhereIF(!filter.Keyword.IsNullOrWhiteSpace(), (dt, ct, dc, ds, dsl, dao, dee, dcol) =>
                                  SqlFunc.ToLower(dt.TableComment).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dt.TableName).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dt.Alias).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dee.Tag).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dcol.ColName).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dcol.ColComment).Contains(filter.Keyword))
                           .GroupBy((dt, ct, dc, ds, dsl, dao, dee) => new
                           {
                               ds.SourceName,
                               dt.SourceId,
                               dt.Id,
                               dt.TableName,
                               dt.Alias,
                               dt.Reviewable,
                               dt.TableComment,
                               dt.Status,
                               dt.UpdateFrequency,
                               dt.UpdateMethod,
                               dt.DataTimeRange,
                               dt.CreateBy,
                               dt.CreateTime,
                               CtlId = dc.Id,
                               CtlCode = dc.Code,
                               CtlName = dc.Name,
                               CtlRemark = dc.Remark,
                               dee.Tag,
                               dee.NeedSup,
                               //dee.NeedSup,
                               dsl.LevelId,
                               dsl.LevelName,
                               dt.QualityScore,
                               dt.LastScore,
                               dt.DataCategory,
                               dt.UpdateCategory,
                           })
                           .Select((dt, ct, dc, ds, dsl, dao, dee) =>
                            new DataTableInfo
                            {
                                Id = dt.Id,
                                SourceId = dt.SourceId,
                                SourceName = ds.SourceName,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                LevelId = dsl.LevelId,
                                LevelName = dsl.LevelName,
                                //OwnerId = SqlFunc.MappingColumn(default(string), "GROUP_CONCAT(DISTINCT dao.owner_id,',')"),
                                //OwnerName = SqlFunc.MappingColumn(default(string), "GROUP_CONCAT(DISTINCT dao.owner_name,',')"),
                                OwnerId = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_id,',')"),
                                OwnerName = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_name,',')"),
                                OwnerDept = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_dept,',')"),//STRING_AGG(DISTINCT CAST(owner_id AS TEXT), ',')
                                CtlId = dc.Id,
                                CtlCode = dc.Code,
                                CtlName = dc.Name,
                                CtlRemark = dc.Remark,
                                //Data = "",//url清单，逗号分割
                                Tag = dee.Tag,
                                NeedSup = dee.NeedSup,
                                UpdateFrequency = dt.UpdateFrequency,
                                UpdateMethod = dt.UpdateMethod,
                                DataTimeRange = dt.DataTimeRange,
                                CreateBy = dt.CreateBy,
                                CreateTime = dt.CreateTime,
                                QualityScore = dt.QualityScore,
                                LastScore = dt.LastScore,
                                DataCategory = dt.DataCategory,
                                UpdateCategory = dt.UpdateCategory,
                            });
            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            var apiList = await _cache.HashGetAllAsync<RouteInfo>(DataAssetManagerConst.RouteRedisKey);
            var hostUrl = DataAssetManagerConst.HostUrl.Replace("http://", "https://");
            list.ForEach(item =>
            {
                item.OwnerId = item.OwnerId?.TrimEnd(',');
                item.OwnerName = item.OwnerName?.TrimEnd(',');
                if (item.OwnerDept.IsNullOrWhiteSpace())
                item.OwnerDept = item.OwnerDept?.TrimEnd(',');
                item.Data = string.Join(",", apiList.Where(f => f.ExecuteConfig?.tableId == item.Id)
                    .Select(f => $"[{f.ReqMethod}]{hostUrl}{f.ApiServiceUrl}").ToArray());
            });
            return new PageResult<DataTableInfo>(query.Count(), list,
                filter.PageNum, filter.PageSize);
        }




        public async Task<List<DataTableInfo>> GetTableByTopic(TopicTableQuery filter)
        {
            List<string> ctlids = new List<string>();
            if (!filter.CtlId.IsNullOrWhiteSpace())
            {
                ctlids.AddRange(filter.CtlId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            if (filter.Keyword.IsNotNullOrWhiteSpace())
            {
                filter.Keyword = filter.Keyword.Trim().ToLower();
            }
            ctlids = ctlids.Distinct().ToList();
            var query = CurrentDb.Queryable<DataTableEntity>()
                           .InnerJoin<DataCatalogTableMapping>((dt, ct) => dt.Id == ct.TableId)
                           .InnerJoin<DataCatalogEntity>((dt, ct, dc) => ct.CatalogId == dc.Id && dc.Status == 1)
                           .InnerJoin<DataSourceEntity>((dt, ct, dc, ds) => dt.SourceId == ds.Id)
                           .LeftJoin<DataSecurityLevelEntity>((dt, ct, dc, ds, dsl)
                                   => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                           .LeftJoin<DataAuthorizeOwnerEntity>((dt, ct, dc, ds, dsl, dao) => dao.ObjectId == dt.Id)
                           .LeftJoin<MetaDataExtEntity>((dt, ct, dc, ds, dsl, dao, dee) => dee.Id == dt.Id)
                           .LeftJoin<DataColumnEntity>((dt, ct, dc, ds, dsl, dao, dee, dcol) => dcol.TableId == dt.Id)
                           .WhereIF(ctlids.Count > 0, (dt, ct, dc, ds, dsl, dao, dee, dcol) => ctlids.Contains(dc.Id))
                           .WhereIF(!filter.Keyword.IsNullOrWhiteSpace(), (dt, ct, dc, ds, dsl, dao, dee, dcol) =>
                                  SqlFunc.ToLower(dt.TableComment).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dt.TableName).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dt.Alias).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dee.Tag).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dcol.ColName).Contains(filter.Keyword) ||
                                  SqlFunc.ToLower(dcol.ColComment).Contains(filter.Keyword))
                           .GroupBy((dt, ct, dc, ds, dsl, dao, dee) => new
                           {
                               ds.SourceName,
                               dt.SourceId,
                               dt.Id,
                               dt.TableName,
                               dt.Alias,
                               dt.Reviewable,
                               dt.TableComment,
                               dt.Status,
                               dt.UpdateFrequency,
                               dt.UpdateMethod,
                               dt.DataTimeRange,
                               dt.CreateBy,
                               dt.CreateTime,
                               CtlId = dc.Id,
                               CtlCode = dc.Code,
                               CtlName = dc.Name,
                               CtlRemark = dc.Remark,
                               dee.Tag,
                               dee.NeedSup,
                               dsl.LevelId,
                               dsl.LevelName,
                               dt.QualityScore,
                               dt.LastScore,
                               dt.DataCategory,
                               dt.UpdateCategory,
                           })
                           .Select((dt, ct, dc, ds, dsl, dao, dee) =>
                            new DataTableInfo
                            {
                                Id = dt.Id,
                                SourceId = dt.SourceId,
                                SourceName = ds.SourceName,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                LevelId = dsl.LevelId,
                                LevelName = dsl.LevelName,
                                OwnerId = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_id,',')"),
                                OwnerName = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_name,',')"),
                                OwnerDept = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_dept,',')"),
                                CtlId = dc.Id,
                                CtlCode = dc.Code,
                                CtlName = dc.Name,
                                CtlRemark = dc.Remark,
                                //Data = "",//url清单，逗号分割
                                Tag = dee.Tag,
                                NeedSup = dee.NeedSup,
                                UpdateFrequency = dt.UpdateFrequency,
                                UpdateMethod = dt.UpdateMethod,
                                DataTimeRange = dt.DataTimeRange,
                                CreateBy = dt.CreateBy,
                                CreateTime = dt.CreateTime,
                                QualityScore = dt.QualityScore,
                                LastScore = dt.LastScore,
                                DataCategory = dt.DataCategory,
                                UpdateCategory = dt.UpdateCategory,
                            });
            return await query.ToListAsync();
        }



        public async Task<PageResult<DataTableInfo>> GetTablesByTag(TopicTableQuery filter)
        {
            var tags = new string[0];
            if (!filter.Tag.IsNullOrWhiteSpace())
            {
                tags = filter.Tag.Split(',', StringSplitOptions.RemoveEmptyEntries);
            }
            var query = CurrentDb.Queryable<DataTableEntity>()
                           .InnerJoin<DataCatalogTableMapping>((dt, ct) => dt.Id == ct.TableId)
                           .InnerJoin<DataCatalogEntity>((dt, ct, dc) => ct.CatalogId == dc.Id && dc.Status == 1)
                           .InnerJoin<DataSourceEntity>((dt, ct, dc, ds) => dt.SourceId == ds.Id)
                           .LeftJoin<DataSecurityLevelEntity>((dt, ct, dc, ds, dsl)
                                   => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                           .LeftJoin<DataAuthorizeOwnerEntity>((dt, ct, dc, ds, dsl, dao) => dao.ObjectId == dt.Id)
                           .InnerJoin<MetaDataExtEntity>((dt, ct, dc, ds, dsl, dao, dee) => dee.Id == dt.Id && tags.Any(d => dee.Tag.Contains(d)))
                           //.WhereIF(filter.OrderField.IsNotNullOrWhiteSpace() && filter.OrderField.Contains("DESC") && filter.OrderField.Contains("QualityScore", StringComparison.CurrentCultureIgnoreCase), (dt, ct, dc, ds, dsl, dao, dee) => dt.QualityScore != null)
                           .WhereIF(filter.DataCategory.IsNotNullOrWhiteSpace(), (dt, ct, dc, ds, dsl, dao, dee) => dt.DataCategory == filter.DataCategory)
                           .GroupBy((dt, ct, dc, ds, dsl, dao, dee) => new
                           {
                               ds.SourceName,
                               dt.SourceId,
                               dt.Id,
                               dt.TableName,
                               dt.Alias,
                               dt.Reviewable,
                               dt.TableComment,
                               dt.Status,
                               CtlId = dc.Id,
                               CtlCode = dc.Code,
                               CtlName = dc.Name,
                               CtlRemark = dc.Remark,
                               dee.Tag,
                               //dee.NeedSup,
                               dsl.LevelId,
                               dsl.LevelName,
                               dt.QualityScore,
                               dt.LastScore,
                               dt.DataCategory,
                               dt.UpdateCategory,
                           })
                           .Select((dt, ct, dc, ds, dsl, dao, dee) =>
                            new DataTableInfo
                            {
                                Id = dt.Id,
                                SourceId = dt.SourceId,
                                SourceName = ds.SourceName,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                LevelId = dsl.LevelId,
                                LevelName = dsl.LevelName,
                                OwnerId = SqlFunc.MappingColumn(default(string), "STRING_AGG(dao.owner_id,',')"),
                                OwnerName = SqlFunc.MappingColumn(default(string), "STRING_AGG( dao.owner_name,',')"),
                                OwnerDept = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_dept,',')"),
                                CtlId = dc.Id,
                                CtlCode = dc.Code,
                                CtlName = dc.Name,
                                CtlRemark = dc.Remark,
                                //Data = "",//url清单，逗号分割
                                Tag = dee.Tag,
                                QualityScore = dt.QualityScore,
                                LastScore = dt.LastScore,
                                DataCategory = dt.DataCategory,
                                UpdateCategory = dt.UpdateCategory,
                                //NeedSup = dee.NeedSup,
                                //UpdateFrequency= "",
                                //UpdateMethod="",
                                //DataTimeRange=""
                                //CreateBy = dt.CreateBy,
                                //CreateTime = dt.CreateTime,
                            });
            if (filter.OrderField.IsNotNullOrWhiteSpace())
            {
                if (filter.OrderField.Contains("QualityScore", StringComparison.CurrentCultureIgnoreCase)) filter.OrderField = " CASE WHEN dt.quality_score IS NULL THEN 1 ELSE 0 END, " + filter.OrderField;
                query = query.OrderBy(filter.OrderField);
            }
            //query = query.OrderByIF(filter.OrderField.IsNotNullOrWhiteSpace(), filter.OrderField);
            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            var apiList = await _cache.HashGetAllAsync<RouteInfo>(DataAssetManagerConst.RouteRedisKey);
            apiList = apiList.Where(f => list.Select(d => d.Id).Contains(f.TableId)).ToList();
            var tableVisited = await GetTableVisited();
            var hostUrl = DataAssetManagerConst.HostUrl.Replace("http://", "https://");
            list.ForEach(item =>
            {
                item.OwnerId = item.OwnerId?.TrimEnd(',');
                item.OwnerName = item.OwnerName?.TrimEnd(',');
                item.Data = string.Join(",", apiList.Where(f => f.ExecuteConfig?.tableId == item.Id)
                    .Select(f => $"[{f.ReqMethod.ToUpper()}]{hostUrl}{f.ApiServiceUrl}").ToArray());
                if (tableVisited.TryGetValue(item.Id, out int visited))
                    item.VisitedTimes = visited;
            });
            return new PageResult<DataTableInfo>(query.Count(), list,
                filter.PageNum, filter.PageSize);
            //return new PageResult<DataTableInfo>(query.Count(), await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync(),
            //filter.PageNum, filter.PageSize);
        }


        public async Task<PageResult<DataTableInfo>> GetUserTable(PageEntity<string> filter)
        {
            var userid = CurrentUser?.UserId ?? "";
            if (string.IsNullOrWhiteSpace(userid)) return new PageResult<DataTableInfo>();
            //if (CurrentUser.IsDataAssetManager) userid = string.Empty;
            List<string> ctlids = new List<string>();
            if (filter.Keyword.IsNotNullOrWhiteSpace())
            {
                filter.Keyword = filter.Keyword.Trim().ToLower();
            }
            var query = CurrentDb.Queryable<DataTableEntity>()
                           .InnerJoin<DataCatalogTableMapping>((dt, ct) => dt.Id == ct.TableId)
                           .InnerJoin<DataCatalogEntity>((dt, ct, dc) => ct.CatalogId == dc.Id && dc.Status == 1)
                           .InnerJoin<DataSourceEntity>((dt, ct, dc, ds) => dt.SourceId == ds.Id)
                           .LeftJoin<DataSecurityLevelEntity>((dt, ct, dc, ds, dsl)
                                   => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                           .LeftJoin<DataAuthorizeOwnerEntity>((dt, ct, dc, ds, dsl, dao) => dao.ObjectId == dt.Id)
                           .LeftJoin<AssetClientScopesEntity>((dt, ct, dc, ds, dsl, dao,dau) => dau.ObjectId == dt.Id)
                           //.LeftJoin<MetaDataExtEntity>((dt, ct, dc, ds, dsl, dao, dee) => dee.Id == dt.Id)
                           //.LeftJoin<DataColumnEntity>((dt, ct, dc, ds, dsl, dao, dee, dcol) => dcol.TableId == dt.Id)
                           .WhereIF(!string.IsNullOrWhiteSpace(userid), (dt, ct, dc, ds, dsl, dao, dau) => dao.OwnerId.Equals(userid)|| dau.ClientId.Equals(userid) || dau.OwnerIds.Contains(userid))
                           .WhereIF(!filter.Keyword.IsNullOrWhiteSpace(), (dt, ct, dc, ds, dsl, dao, dau) =>
                                 SqlFunc.ToLower( dt.TableComment).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.TableName).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.Alias).Contains(filter.Keyword))
                           .GroupBy((dt, ct, dc, ds, dsl, dao, dau) => new
                           {
                               ds.SourceName,
                               dt.SourceId,
                               dt.Id,
                               dt.TableName,
                               dt.Alias,
                               dt.Reviewable,
                               dt.TableComment,
                               dt.Status,
                               CtlId = dc.Id,
                               CtlCode = dc.Code,
                               CtlName = dc.Name,
                               CtlRemark = dc.Remark,
                               //dee.Tag,
                               //dee.NeedSup,
                               dsl.LevelId,
                               dsl.LevelName,
                               dt.QualityScore,
                               dt.LastScore,
                               dt.DataCategory,
                               dt.UpdateCategory,
                           })
                           .Select((dt, ct, dc, ds, dsl, dao, dau) =>
                            new DataTableInfo
                            {
                                Id = dt.Id,
                                SourceId = dt.SourceId,
                                SourceName = ds.SourceName,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                LevelId = dsl.LevelId,
                                LevelName = dsl.LevelName,
                                OwnerId = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_id,',')"),
                                OwnerName = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_name,',')"),
                                OwnerDept = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_dept,',')"),
                                CtlId = dc.Id,
                                CtlCode = dc.Code,
                                CtlName = dc.Name,
                                CtlRemark = dc.Remark,
                                QualityScore = dt.QualityScore,
                                LastScore = dt.LastScore,
                                DataCategory = dt.DataCategory,
                                UpdateCategory = dt.UpdateCategory,
                                //Data = "",//url清单，逗号分割
                                //Tag = dee.Tag,
                                //NeedSup = dee.NeedSup,
                                //UpdateFrequency = "",
                                //UpdateMethod = "",
                                //DataTimeRange = ""
                                //CreateBy = dt.CreateBy,
                                //CreateTime = dt.CreateTime,
                            });
            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            var apiList = await _cache.HashGetAllAsync<RouteInfo>(DataAssetManagerConst.RouteRedisKey);
            apiList = apiList.Where(f => list.Select(d => d.Id).Contains(f.TableId)).ToList();
            var tableVisited = await GetTableVisited();
            var hostUrl = DataAssetManagerConst.HostUrl.Replace("http://", "https://");
            list.ForEach(item =>
            {
                item.OwnerId = item.OwnerId?.TrimEnd(',');
                item.OwnerName = item.OwnerName?.TrimEnd(',');
                item.Data = string.Join(",", apiList.Where(f => f.ExecuteConfig?.tableId == item.Id)
                    .Select(f => $"[{f.ReqMethod.ToUpper()}]{hostUrl}{f.ApiServiceUrl}").ToArray());
                if (tableVisited.TryGetValue(item.Id, out int visited))
                    item.VisitedTimes = visited;
            });
            return new PageResult<DataTableInfo>(query.Count(), list,
                filter.PageNum, filter.PageSize);
        }

        public async Task<List<DataTableInfo>> GetUserAuthTablesByUserId(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid)) return new List<DataTableInfo>();
            List<string> ctlids = new List<string>();
            var query = CurrentDb.Queryable<DataAuthorizeUserEntity>()
                           .InnerJoin<DataTableEntity>((dau, dt) => dt.Id == dau.ObjectId)
                           .WhereIF(!string.IsNullOrWhiteSpace(userid), (dau, dt) => dau.UserId.Equals(userid))
                           .Select((dau, dt) =>
                            new DataTableInfo
                            {
                                Id = dt.Id,
                                SourceId = dt.SourceId,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                CtlId = dau.CtlId,
                            });
            return await query.ToListAsync();
        }

        public async Task<PageResult<DataTableInfo>> GetOwnerTables(DataTableDto filter)
        {
            var userid = CurrentUser?.UserId ?? "";
            if (string.IsNullOrWhiteSpace(userid)) return new PageResult<DataTableInfo>();
            //if (CurrentUser.IsDataAssetManager) userid = string.Empty;
            List<string> ctlids = new List<string>();
            if (filter.Keyword.IsNotNullOrWhiteSpace())
            {
                filter.Keyword = filter.Keyword.Trim().ToLower();
            }
            var query = CurrentDb.Queryable<DataTableEntity>()
                           //.InnerJoin<DataCatalogTableMapping>((dt, ct) => dt.Id == ct.TableId)
                           //.InnerJoin<DataCatalogEntity>((dt, ct, dc) => ct.CatalogId == dc.Id && dc.Status == 1)
                           .InnerJoin<DataSourceEntity>((dt, ds) => dt.SourceId == ds.Id)
                           //.LeftJoin<DataSecurityLevelEntity>((dt,  ds, dsl)
                           //        => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                           .LeftJoin<DataAuthorizeOwnerEntity>((dt, ds, dao) => dao.ObjectId == dt.Id)
                           .WhereIF(!string.IsNullOrWhiteSpace(userid), (dt, ds, dao) => dao.OwnerId.Equals(userid))
                           .WhereIF(!filter.Keyword.IsNullOrWhiteSpace(), (dt, ds, dao) =>
                                 SqlFunc.ToLower(dt.TableComment).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.TableName).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.Alias).Contains(filter.Keyword))
                           .GroupBy((dt, ds, dao) => new
                           {
                               ds.SourceName,
                               dt.SourceId,
                               dt.Id,
                               dt.TableName,
                               dt.Alias,
                               dt.Reviewable,
                               dt.TableComment,
                               dt.Status,
                               dt.QualityScore,
                               dt.LastScore,
                               dt.DataCategory,
                               dt.UpdateCategory,
                           })
                           .Select((dt, ds, dao) =>
                            new DataTableInfo
                            {
                                Id = dt.Id,
                                SourceId = dt.SourceId,
                                SourceName = ds.SourceName,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                OwnerId = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_id,',')"),
                                OwnerName = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_name,',')"),
                                OwnerDept = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dao.owner_dept,',')"),
                                QualityScore = dt.QualityScore,
                                LastScore = dt.LastScore,
                                DataCategory = dt.DataCategory,
                                UpdateCategory = dt.UpdateCategory,
                            });
            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            var tableVisited = await GetTableVisited();
            list.ForEach(item =>
            {
                item.OwnerId = item.OwnerId?.TrimEnd(',');
                item.OwnerName = item.OwnerName?.TrimEnd(',');
                if (tableVisited.TryGetValue(item.Id, out int visited))
                    item.VisitedTimes = visited;
            });
            return new PageResult<DataTableInfo>(query.Count(), list, filter.PageNum, filter.PageSize);
        }


        #region Save
        public async Task<DataTableEntity> Save(DataTableInput entity)
        {
            if (entity.ColumnList != null && entity.ColumnList.Any(f => f.ColComment.IsNullOrWhiteSpace()))
            {
                throw new Furion.FriendlyException.AppFriendlyException("column comment cannot be null", "5001");
            }
            using (var uow = CurrentDb.CreateContext())
            {
                var model = entity.Adapt<DataTableEntity>();// _mapper.Map<DataTableInput, DataTableEntity>(entity);
                                                            //if (model.Id.IsNullOrWhiteSpace()) entity.Id = model.Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true }).ToString();
                                                            
                //获取表格的最高安全级别
                model.LevelId =entity.ColumnList.Max(f => f.LevelId);
                model.LevelName = model.LevelId.GetEnum<SecurityLevel>().GetDescription();
                var result = await Create(model, false);
                entity.Id = model.Id;
                if (result <= 0) return model;

                result = await SaveOwners(entity);
                result = await SaveSecurityLevel(entity);
                result = await SaveColumns(entity, model);
                result = await SaveDataUser(entity, model);
                result = await SaveExt(entity, model);
                uow.Commit();
                await RefreshCache();
                return model;
            }
        }

        private async Task<int> SaveOwners(DataTableInput entity)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (entity.OwnerList != null)
                {
                    foreach (var item in entity.OwnerList)
                    {
                        item.ObjectId = entity.Id;
                        item.ObjectType = "table";
                        await Create(item, false);
                    }
                    //(async f =>
                    //{
                    //    //f.Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true }).ToString();

                    //});
                    //return await CurrentDb.Storageable(entity.OwnerList).ExecuteSqlBulkCopyAsync();
                }
                uow.Commit();
                return await Task.FromResult(1);
            }
        }

        private async Task<int> SaveSecurityLevel(DataTableInput entity)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var result = 1;
                //获取表格的最高安全级别
                entity.LevelId = entity.ColumnList.Max(f => f.LevelId);
                entity.LevelName = entity.LevelId.GetEnum<SecurityLevel>().GetDescription();
                if (!entity.LevelId.IsNullOrWhiteSpace())
                {
                    result = await Create(new DataSecurityLevelEntity()
                    {
                        //Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true }).ToString(),
                        LevelId = entity.LevelId,
                        LevelName = entity.LevelName,
                        ObjectId = entity.Id,
                        ObjectType = "table",
                    }, false);
                }
                uow.Commit();
                return 1;
            }
        }

        private async Task<int> SaveColumns(DataTableInput entity, DataTableEntity model)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (entity.ColumnList != null)
                {
                    var columns = entity.ColumnList.Adapt<List<DataColumnEntity>>();// _mapper.Map<List<DbColumn>, List<DataColumnEntity>>(entity.ColumnList);
                    foreach (var item in columns)
                    {
                        item.TableId = model.Id;
                        item.SourceId = model.SourceId;
                        await Create(item, false);
                    }
                    //columns.ForEach(async item =>
                    //{
                    //    //item.Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true, }).ToString();
                    //    item.TableId = model.Id;
                    //    item.SourceId = model.SourceId;
                    //    await Create(item);
                    //});
                    //return await CurrentDb.Storageable(columns).ExecuteSqlBulkCopyAsync();
                }
                uow.Commit();
                return 1;
            }
        }

        private async Task<int> SaveDataUser(DataTableInput entity, DataTableEntity model)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (entity.ApproverList != null)
                {
                    var dataUser = new List<MetaDataUserEntity>();
                    var list = entity.ApproverList.OrderBy(f => f.Sort);
                    var index = 1;
                    foreach (var item in list)
                    {
                        foreach (var user in item.UserList)
                        {
                            await Create(new MetaDataUserEntity()
                            {
                                //Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true, }).ToString(),
                                ObjectId = model.Id,
                                ObjectType = "table",
                                UserType = "approver",
                                UserId = user.UserId,
                                UserName = user.UserName,
                                Sort = index++,
                            }, false);
                        }
                        //item.UserList.ForEach(async user =>
                        //{
                        //    await Create(new MetaDataUserEntity()
                        //    {
                        //        //Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true, }).ToString(),
                        //        ObjectId = model.Id,
                        //        ObjectType = "table",
                        //        UserType = "approver",
                        //        UserId = user.UserId,
                        //        UserName = user.UserName,
                        //        Sort = index++,
                        //    });
                        //    //dataUser.Add(new MetaDataUserEntity()
                        //    //{
                        //    //    Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true, }).ToString(),
                        //    //    ObjectId = model.Id,
                        //    //    ObjectType = "table",
                        //    //    UserType = "approver",
                        //    //    UserId = user.UserId,
                        //    //    UserName = user.UserName,
                        //    //    Sort = index++,
                        //    //});
                        //});
                    };
                    //return await CurrentDb.Storageable(dataUser).ExecuteSqlBulkCopyAsync();
                }
                uow.Commit();
                return await Task.FromResult(1);
            }
        }

        private async Task<int> SaveExt(DataTableInput entity, DataTableEntity model)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var result = 1;
                if (!entity.Tag.IsNullOrWhiteSpace())
                {
                    result = await Create(new MetaDataExtEntity()
                    {
                        Id = model.Id,
                        Tag = entity.Tag,
                        NeedSup = entity.NeedSup,
                        ObjectType = "table",
                    }, false);
                }
                uow.Commit();
                return result;
            }
        }

        #endregion Save


        #region Update
        public async Task<DataTableEntity> Update(DataTableInput entity)
        {
            if (entity.ColumnList != null && entity.ColumnList.Any(f => f.ColComment.IsNullOrWhiteSpace()))
            {
                throw new Exception("column comment cannot be null");
            }
            using (var uow = CurrentDb.CreateContext())
            {
                var oldEntity = await Get(entity.Id);
                var model = entity.Adapt<DataTableEntity>();// _mapper.Map<DataTableInput, DataTableEntity>(entity);
                if (model.Id.IsNullOrWhiteSpace()) throw new Exception("Id cannot be null");
                var result = await Modify(model,false);
                if (result <= 0) return model;

                result = await UpdateOwners(entity);
                result = await UpdateSecurityLevel(entity);
                result = await UpdateColumns(entity, model);
                result = await UpdateDataUser(entity, model);
                result = await UpateExt(entity, model);

                if (oldEntity.SourceId != model.SourceId || !oldEntity.TableName.Equals(model.TableName, StringComparison.CurrentCultureIgnoreCase))
                {//修改了数据源，或者表名，需要更新api信息
                    await UpdateTableApi(model, oldEntity);
                }

                uow.Commit();
                await RefreshCache();
                return model;
            }
        }

        private async Task UpdateTableApi(DataTableEntity newEntity,DataTableEntity oldEntity)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var apiList = await CurrentDb.Queryable<DataApiEntity>().Where(f => f.TableId == newEntity.Id).ToListAsync();
                foreach (var item in apiList)
                {
                    item.SourceId = newEntity.SourceId;
                    item.ExecuteConfig.sourceId = newEntity.SourceId;
                    item.ExecuteConfig.tableName = newEntity.TableName;
                    if (!oldEntity.TableName.Equals(newEntity.TableName, StringComparison.CurrentCultureIgnoreCase)
                        && item.ExecuteConfig.configType != "3" && item.ExecuteConfig.sqlText.IsNotNullOrWhiteSpace() && item.ExecuteConfig.sqlText.Contains(oldEntity.TableName))
                    {
                        item.ExecuteConfig.sqlText = item.ExecuteConfig.sqlText.Replace(oldEntity.TableName, newEntity.TableName);
                    }
                    await Modify(item);
                }
                uow.Commit();
            }
        }

        private async Task<int> UpdateOwners(DataTableInput entity)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var oldOwners = await CurrentDb.Queryable<DataAuthorizeOwnerEntity>().Where(f => f.ObjectId == entity.Id).ToListAsync();
                if (!VerifyOwner(oldOwners, entity.OwnerList))
                {
                    await CurrentDb.Deleteable<DataAuthorizeOwnerEntity>().Where(f => f.ObjectId == entity.Id).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
                    await SaveOwners(entity);
                }
                uow.Commit();
                return 1;
            }
        }

        private async Task<int> UpdateSecurityLevel(DataTableInput entity)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                //获取表格的最高安全级别
                entity.LevelId = entity.ColumnList.Max(f => f.LevelId);
                entity.LevelName = entity.LevelId.GetEnum<SecurityLevel>().GetDescription();
                if (!entity.LevelId.IsNullOrWhiteSpace())
                {
                    var data = await CurrentDb.Queryable<DataSecurityLevelEntity>().Where(f => f.ObjectId == entity.Id && f.ObjectType == "table").FirstAsync();
                    if (data != null)
                    {
                        data.LevelId = entity.LevelId;
                        data.LevelName = entity.LevelName;
                        await ModifyHasChange(data, false);
                    }
                    else
                    {
                        await SaveSecurityLevel(entity);
                    }
                }
                uow.Commit();
                return 1;
            }
        }
        private async Task<int> UpdateColumns(DataTableInput entity, DataTableEntity model)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (entity.ColumnList != null)
                {
                    var oldColumns = await _columnService.GetByTableId(model.Id);
                    var delIds = new List<string>();
                    foreach (var column in oldColumns)
                    {
                        var data = entity.ColumnList.FirstOrDefault(f => f.Id == column.Id);
                        if (data != null)
                        {
                            column.Id = data.Id;
                            await ModifyHasChange(data.Adapt<DataColumnEntity>(), false);
                        }
                        else
                        {
                            delIds.Add(column.Id);
                        }
                    }

                    var newColumns = entity.ColumnList.Where(f => f.Id.IsNullOrWhiteSpace() || (!f.Id.IsNullOrWhiteSpace() && !oldColumns.Select(d => d.Id).Contains(f.Id))).ToList();
                    foreach (var d in newColumns)
                    {
                        var item = d.Adapt<DataColumnEntity>();
                        item.Id = _idGenerator.Create(new SequentialGuidSettings() { LittleEndianBinary16Format = true, }).ToString();
                        item.TableId = model.Id;
                        item.SourceId = model.SourceId;
                        await Create(item, false);
                    }

                    var result = await CurrentDb.Deleteable<DataColumnEntity>().In(delIds).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
                    foreach (var id in delIds)
                    {
                        await _cache.DelayRemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(DataColumnEntity).Name}:{id}");
                    }

                }
                uow.Commit();
                return 1;
            }
        }


        private async Task<int> UpdateDataUser(DataTableInput entity, DataTableEntity model)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var list = await _dataUserService.GetTableApprovers(model.Id);
                var index = 1;
                var approverList = entity.ApproverList.OrderBy(f => f.Sort).ToList();
                approverList.ForEach(f => f.Sort = index++);
                if (VerifyUser(list, approverList))
                {
                    await CurrentDb.Deleteable<MetaDataUserEntity>()
                        .Where(f => f.ObjectType == "table" && f.UserType == "approver" && f.ObjectId == model.Id).EnableDiffLogEventIF(EnableDiffLogEvent)
                        .ExecuteCommandAsync();
                    await SaveDataUser(entity, model);
                }
                uow.Commit();
                return 1;
            }
        }

        private async Task<int> UpateExt(DataTableInput entity, DataTableEntity model)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (!entity.Tag.IsNullOrWhiteSpace())
                {
                    var data = await CurrentDb.Queryable<MetaDataExtEntity>().Where(f => f.Id == model.Id).FirstAsync();
                    if (data == null) await SaveExt(entity, model);
                    else
                    {
                        data.Tag = entity.Tag;
                        data.NeedSup = entity.NeedSup;
                        await ModifyHasChange(data, false);
                    }
                }
                uow.Commit();
                return 1;
            }
        }


        /// <summary>
        /// 校验数据拥有者是否发生变化
        /// </summary>
        /// <param name="oldList">旧的数据拥有者列表</param>
        /// <param name="newList">新的数据拥有者列表</param>
        /// <returns>是否一致</returns>
        private bool VerifyOwner(List<DataAuthorizeOwnerEntity> oldList, List<DataAuthorizeOwnerEntity> newList)
        {
            // 如果新列表为空，则不一致,但是不删除原先数据
            if (newList.Count == 0)
            {
                return true;
            }

            // 获取旧的数据拥有者ID集合
            var oldSet = oldList.Select(o => o.OwnerId).ToHashSet();

            // 获取新的数据拥有者ID集合
            var newSet = newList.Select(o => o.OwnerId).ToHashSet();

            // 如果新旧集合不一致，则不一致
            if (!newSet.SetEquals(oldSet))
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 校验多级审批人是否一致
        /// </summary>
        /// <param name="userMap">旧的审批人分组</param>
        /// <param name="approverNews">新的审批人列表</param>
        /// <returns>是否一致</returns>
        public bool VerifyUser(List<ApproveDto> userMap, List<ApproveDto> approverNews)
        {
            // 如果两者都为空，则一致
            if (userMap.Count == 0 && approverNews.Count == 0)
            {
                return true;
            }

            // 如果分组数量不一致，则不一致
            if (userMap.Count != approverNews.Count)
            {
                return false;
            }

            // 遍历新的审批人列表
            foreach (var approveDto in approverNews)
            {
                int sort = approveDto.Sort;
                // 获取新的审批人ID集合
                var newSet = approveDto.UserList.Select(o => o.UserId).ToHashSet();
                // 获取旧的审批人ID集合
                var oldSet = userMap.FirstOrDefault(f => f.Sort == sort)?.UserList.Select(o => o.UserId).ToHashSet();

                // 如果新旧集合不一致，则不一致
                if (!newSet.SetEquals(oldSet))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion Update

        public async Task<DataTableInput> GetInfo(string id)
        {
            var tableEntity =  (await AllFromCache()).FirstOrDefault(f => f.Id == id);
            if(tableEntity == null)return null;
            var entity = tableEntity.Adapt<DataTableInput>();
            entity.OwnerList = await CurrentDb.Queryable<DataAuthorizeOwnerEntity>().Where(f => f.ObjectId == id && f.ObjectType == "table").ToListAsync();
            entity.ColumnList = (await _columnService.GetByTableId(entity.Id)).Adapt<List<DataColumnInfo>>();
            entity.ApproverList = await _dataUserService.GetTableApprovers(entity.Id);
            var extInfo = await CurrentDb.Queryable<MetaDataExtEntity>().Where(f => f.Id == id && f.ObjectType == "table").FirstAsync();
            if (extInfo != null)
            {
                entity.NeedSup = extInfo.NeedSup;
                entity.Tag = extInfo.Tag;
            }
            if (entity.OwnerDepart.IsNullOrWhiteSpace()&& entity.OwnerList.Count()>0)
            {
                entity.OwnerDepart = string.Join(";", entity.OwnerList.Where(f => f.OwnerDept.IsNotNullOrWhiteSpace()).Select(f => f.OwnerDept).Distinct().ToArray());
            }
            return entity;
        }


        public override async Task<PageResult<DataTableEntity>> Page(DataTableInfo filter)
        {
            filter.Keyword = filter.Keyword?.Trim()?.ToLower();
            filter.SourceName = filter.SourceName?.Trim()?.ToLower();
            filter.TableName = filter.TableName?.Trim()?.ToLower();
            filter.OwnerDept = filter.OwnerDept?.Trim()?.ToLower();
            filter.OwnerId = filter.OwnerId?.Trim()?.ToLower();
            var query = CurrentDb.Queryable<DataTableEntity>()
                           .InnerJoin<DataSourceEntity>((dt, ds) => dt.SourceId == ds.Id)
                           .LeftJoin<DataSecurityLevelEntity>((dt, ds, dsl)
                                   => dt.Id == dsl.ObjectId && dsl.ObjectType == "table")
                           .LeftJoin<DataAuthorizeOwnerEntity>((dt, ds, dsl, o)
                                   => dt.Id == o.ObjectId && o.ObjectType == "table")
                           .WhereIF(!filter.UpdateMethod.IsNullOrWhiteSpace(), (dt, ds, dsl, o) => dt.UpdateMethod == filter.UpdateMethod)
                           .WhereIF(!filter.LevelId.IsNullOrWhiteSpace(), (dt, ds, dsl, o) => dsl.LevelId == filter.LevelId)
                           .WhereIF(!filter.OwnerId.IsNullOrWhiteSpace(), (dt, ds, dsl, o) => o.OwnerId == filter.OwnerId)
                           .WhereIF(filter.Status.HasValue, (dt, ds, dsl, o) => dt.Status == filter.Status)
                           .WhereIF(!filter.SourceId.IsNullOrWhiteSpace(), (dt, ds, dsl, o) => dt.SourceId == filter.SourceId)
                           .WhereIF(!filter.SourceName.IsNullOrWhiteSpace(), (dt, ds, dsl, o) => SqlFunc.ToLower(ds.SourceName).Equals(filter.SourceName))
                           .WhereIF(!filter.OwnerDept.IsNullOrWhiteSpace(), (dt, ds, dsl, o) => SqlFunc.ToLower(dt.OwnerDepart).Equals(filter.OwnerDept)|| SqlFunc.ToLower(o.OwnerDept).Equals(filter.OwnerDept))
                           .WhereIF(!filter.Keyword.IsNullOrWhiteSpace(), (dt, ds, dsl, o) =>
                                    SqlFunc.ToLower(dt.TableComment).Contains(filter.Keyword) ||
                                    SqlFunc.ToLower(dt.TableName).Contains(filter.Keyword) ||
                                    SqlFunc.ToLower(dt.Alias).Contains(filter.Keyword))
                           .WhereIF(!filter.TableName.IsNullOrWhiteSpace(), (dt, ds, dsl, o) =>
                                    SqlFunc.ToLower(dt.TableComment).Contains(filter.TableName) ||
                                    SqlFunc.ToLower(dt.TableName).Contains(filter.TableName) ||
                                    SqlFunc.ToLower(dt.Alias).Contains(filter.TableName))
                           .OrderByDescending((dt, ds, dsl, o) => dt.CreateTime)
                           .Select((dt, ds, dsl, o) =>
                            new DataTableEntity
                            {
                                Id = dt.Id,
                                SourceId = ds.Id,
                                SourceName = ds.SourceName,
                                TableName = dt.TableName,
                                Alias = dt.Alias,
                                Reviewable = dt.Reviewable,
                                TableComment = dt.TableComment,
                                Status = dt.Status,
                                LevelId = dsl.LevelId,
                                LevelName = dsl.LevelName,
                                UpdateFrequency = dt.UpdateFrequency,
                                UpdateMethod = dt.UpdateMethod,
                                DataTimeRange = dt.DataTimeRange,
                                CreateBy = dt.CreateBy,
                                CreateTime = dt.CreateTime,
                                UpdateBy = dt.UpdateBy,
                                UpdateTime = dt.UpdateTime,
                                OwnerId = o.OwnerId,
                                OwnerName = o.OwnerName,
                                OwnerDepart = dt.OwnerDepart ?? o.OwnerDept,
                                QualityScore = dt.QualityScore,
                                LastScore = dt.LastScore,
                                DataCategory = dt.DataCategory,
                                UpdateCategory = dt.UpdateCategory,
                                Remark = dt.Remark,
                            }).OrderByDescending(dt => dt.CreateTime);
            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            return new PageResult<DataTableEntity>(query.Count(), list, filter.PageNum, filter.PageSize);
        }



        public async override Task<bool> Delete(string id,bool clearCache=true)
        {
            var result = await base.Delete(id);
            var iresult = await CurrentDb.Deleteable<DataAuthorizeOwnerEntity>().Where(f => f.ObjectId == id && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            iresult = await CurrentDb.Deleteable<DataSecurityLevelEntity>().Where(f => f.ObjectId == id && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();

            iresult = await _columnService.DeleteByTableId(id);
            iresult = await _dataUserService.DeleteByTableId(id);
            iresult = await CurrentDb.Deleteable<MetaDataExtEntity>().Where(f => f.Id == id && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();

            await RefreshCache();
            return result;
        }

        public async override Task<bool> Delete(dynamic[] ids, bool clearCache = true)
        {
            if (ids.Length == 0) return true;
            using (var uow = CurrentDb.CreateContext())
            {
                var result = await base.Delete(ids);
                var iresult = await CurrentDb.Deleteable<DataAuthorizeOwnerEntity>().Where(f => ids.Contains(f.ObjectId) && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
                iresult = await CurrentDb.Deleteable<DataSecurityLevelEntity>().Where(f => ids.Contains(f.ObjectId) && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();

                iresult = await _columnService.DeleteByTableId(ids);
                iresult = await _dataUserService.DeleteByTableId(ids);
                iresult = await CurrentDb.Deleteable<MetaDataExtEntity>().Where(f => ids.Contains(f.Id) && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
                uow.Commit();
                await RefreshCache();
                return result;
            }
        }


        public async Task<List<string>> GetTableDepts()
        {
            var fun = async () =>
            {
                return await CurrentDb.Queryable<DataTableEntity>().Where(f => f.OwnerDepart != null).Select(f => f.OwnerDepart).Distinct().ToListAsync();
            };
            return await _cache.GetObjectAsync(GetRedisKey<DataTableEntity>("TableDepts"), fun, TimeSpan.FromMinutes(1));
        }

        public async Task<List<TableApiVisitStatics>> GetTableStatistics()
        {
            var fun = async () =>
            {
                var logQuery = CurrentDb.Queryable<DataApiLogView>().Where(lg => lg.CallerDate > DateTimeOffset.Now.AddDays(-30));
                //var tableQuery = CurrentDb.Queryable<DataTableEntity>().Select(t => new { t.Id, t.TableName, t.TableComment, t.OwnerDepart });
                return await logQuery//.InnerJoin(tableQuery, (lg, t) => lg.TableId == t.Id)
                     .GroupBy((lg) => new { lg.TableName, lg.TableComment, lg.OwnerDepart })
                     .Select((lg) => new TableApiVisitStatics()
                     {
                         Dept = lg.OwnerDepart,
                         TableCode = lg.TableName,
                         TableName = lg.TableComment,
                         Count = SqlFunc.AggregateDistinctCount(lg.Id)
                     })
                     .OrderByDescending(lg => lg.Count).Take(50).ToListAsync();
            };
            return await _cache.GetObjectAsync(DataAssetManagerConst.DataTable_VisitedStatisticsKey, fun, TimeSpan.FromMinutes(10));
        }

        public async Task<List<TableStandardizedRateEntity>> GetStandardizationStatistics(string dept)
        {
            var fun = async () =>
            {
                return await CurrentDb.Queryable<TableStandardizedRateEntity>().Where(f => f.Dept != null)
                .WhereIF(dept.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Dept) == dept.ToLower())
                .ToListAsync();
            };
            return await _cache.GetObjectAsync(GetRedisKey<TableStandardizedRateEntity>("StandardRate", dept), fun, TimeSpan.FromMinutes(1));
        }


        public override async Task RefreshCache<TEntity>()
        {
            await base.RefreshCache<TEntity>();
            await base.RefreshCache<DataAuthorizeOwnerEntity>();
            await base.RefreshCache<DataSecurityLevelEntity>();
            await base.RefreshCache<MetaDataExtEntity>();
            await _eventPublisher.PublishAsync(new ChannelEventSource(DataAssetManagerConst.DataTable_UserHashKey));
            //await _eventPublisher.PublishAsync(new ChannelEventSource(DataAssetManagerConst.DataApis_HashKey));
            //await InitRedisHash();
            //await InitTableUserFromCache(true);
            //var _ = Task.Run();
            //var _2 = Task.Run(async () => {  });
        }

        /// <summary>
        /// map到category上的数量
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetCategoryMapTableCount()
        {
            var catalogQuery = CurrentDb.Queryable<DataCatalogEntity>().Where(f => f.Status == 1)
                .InnerJoin<DataCatalogTableMapping>((f, t) => f.Id == t.CatalogId)
                .InnerJoin<DataTableEntity>((f, t, dt) => t.TableId == dt.Id)
                .Where((f, t, dt) => dt.Id != null && dt.Status != 0)
                .Select((f, t) => t.TableId).Distinct();
            return await catalogQuery.CountAsync();
        }

        /// <summary>
        /// map到category上的数量
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetCategoryMapTableIdList()
        {
            var catalogQuery = CurrentDb.Queryable<DataCatalogEntity>().Where(f => f.Status == 1)
                .InnerJoin<DataCatalogTableMapping>((f, t) => f.Id == t.CatalogId)
                .Select((f, t) => t.TableId).Distinct();
            return await catalogQuery.ToListAsync();
        }


        /// <summary>
        /// map到category上的数量
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetCategoryMapColumnCount()
        {
            var catalogQuery = CurrentDb.Queryable<DataCatalogEntity>().Where(f => f.Status == 1)
                .InnerJoin<DataCatalogTableMapping>((f, t) => f.Id == t.CatalogId)
                .InnerJoin<DataColumnEntity>((f, t, c) => t.TableId == c.TableId)
                .Select((f, t, c) => c.Id).Distinct();
            return await catalogQuery.CountAsync();
        }

        /// <summary>
        /// map到category上的数量
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetCategoryMapApiCount()
        {
            var catalogQuery = CurrentDb.Queryable<DataCatalogEntity>().Where(f => f.Status == 1)
                .InnerJoin<DataCatalogTableMapping>((f, t) => f.Id == t.CatalogId)
                .InnerJoin<DataApiEntity>((f, t, c) => t.TableId == c.TableId)
                .Select((f, t, c) => c.Id).Distinct();
            return await catalogQuery.CountAsync();
        }

        /// <summary>
        /// map到category上的数量
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetCategoryMapApiList()
        {
            var catalogQuery = CurrentDb.Queryable<DataCatalogEntity>().Where(f => f.Status == 1)
                .InnerJoin<DataCatalogTableMapping>((f, t) => f.Id == t.CatalogId)
                .InnerJoin<DataApiEntity>((f, t, c) => t.TableId == c.TableId)
                .Select((f, t, c) => c.Id).Distinct();
            return await catalogQuery.ToListAsync();
        }


        //public override Task<int> Modify<TEntity>(TEntity entity)
        //{
        //    if (entity is DataTableEntity)
        //    {
        //        var model = entity as DataTableEntity;
        //        model.UpdateBy = CurrentUser?.UserName ?? "test";
        //        model.UpdateTime =  DateTimeOffset.Now;
        //    }
        //    return base.Modify(entity);
        //}
        //public override Task<bool> ModifyHasChange<TEntity>(TEntity entity)
        //{
        //    if (entity is DataTableEntity)
        //    {
        //        var model = entity as DataTableEntity;
        //        model.UpdateBy = CurrentUser?.UserName ?? "test";
        //        model.UpdateTime = DateTimeOffset.Now;
        //    }
        //    return base.ModifyHasChange(entity);
        //}
    }
}
