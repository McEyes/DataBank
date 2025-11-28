using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;
using Furion.DatabaseAccessor;
using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Encrypt;
using ITPortal.Core.Extensions;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using System.Text;

using IResult = ITPortal.Core.Services.IResult;

namespace DataAssetManager.DataApiServer.Application
{
    public class AssetClientsService : BaseService<AssetClientEntity, AssetClientQueryDto, Guid>, IAssetClientsService, ITransient
    {
        private readonly IDataTableService _tableService;
        private readonly IEmployeeBaseInfoService _userProxyService;
        private readonly IEventPublisher _eventPublisher;
        public AssetClientsService(ISqlSugarClient db, IDistributedCacheService cache, IDataTableService tableService, IEmployeeBaseInfoService userProxyService, IEventPublisher eventPublisher)   : base(db, cache)
        {
            _tableService = tableService;
            _userProxyService = userProxyService;
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<AssetClientEntity> BuildFilterQuery(AssetClientQueryDto filter)
        {
            filter.Keyword = filter.Keyword?.ToLower();
            if (filter.Owner == null) filter.Owner = "";
            return CurrentDb.Queryable<AssetClientEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Keyword), f =>SqlFunc.ToLower( f.ClientId) == filter.Keyword || SqlFunc.ToLower(f.ClientName).Contains(filter.Keyword) || SqlFunc.ToLower(f.Description).Contains(filter.Keyword) || SqlFunc.ToLower(f.NickName).Contains(filter.Keyword))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ClientId), f => SqlFunc.ToLower(f.ClientId) == filter.ClientId.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ClientName), f => SqlFunc.ToLower(f.ClientName).Contains(filter.ClientName.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Owner), f => SqlFunc.ToLower(f.Owner).Contains(filter.Owner.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.OwnerDept), f => SqlFunc.ToLower(f.OwnerDept).Contains(filter.OwnerDept.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Sme), f =>SqlFunc.JsonLike(f.SMEList, filter.Sme.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.BelongArea), f => SqlFunc.ToLower(f.BelongArea).Contains(filter.BelongArea.ToLower()))
                .WhereIF(filter.ClientType.HasValue, f => f.ClientType == filter.ClientType)
                .WhereIF(filter.Enabled.HasValue, f => f.Enabled == filter.Enabled)
                .OrderByDescending(f => f.CreateTime);
        }

        #region Clients

        public async Task<List<AssetClientDto>> AllClients()
        {
            return (await CurrentDb.Queryable<AssetClientEntity>().Where(f => f.Enabled == true).OrderByDescending(f => f.CreateTime).ToListAsync()).Adapt<List<AssetClientDto>>();
        }


        public async Task<List<AssetClientDto>> AllClientsFromCache(bool clearCache = false)
        {
            var key = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:List";
            //if (clearCache) await _cache.RemoveAsync(key);
            return await _cache.GetObjectAsync(key, AllClients, null, clearCache);
        }


        public async Task<PageResult<AssetClientView>> QueryClientAndSecrets(AssetClientQueryDto filter)
        {
            var keyword = filter.Keyword?.ToLower();
            filter.Keyword = string.Empty;
            var query = BuildFilterQuery(filter);
            var allQuery = query
                 .InnerJoin(CurrentDb.Queryable<AssetClientSecretsEntity>().Select(s => new { s.ClientUId, s.Secrets }), (f, s) => f.Id == s.ClientUId)
                 .InnerJoin(CurrentDb.Queryable<AssetClientScopesEntity>().WhereIF(keyword.IsNotNullOrWhiteSpace(),sc=>sc.SearchData.Contains(keyword)|| SqlFunc.ToLower(sc.ClientId) == keyword || SqlFunc.ToLower(sc.ClientName).Contains(keyword) || SqlFunc.ToLower(sc.Description).Contains(keyword)).Select(sc => new { sc.ClientUId, sc.FlowNo, sc.Description, sc.ObjectId, sc.CreateTime }),
                 (f, s, sc) => f.Id == sc.ClientUId)
                 .OrderByDescending((f, s, sc) =>sc.CreateTime)
                 .Select((f, s, sc) => new AssetClientView()
                 {
                     //Id = f.Id,
                     ClientId = f.ClientId,
                     ClientName = f.ClientName,
                     ClientType = f.ClientType,
                     Description = f.Description,
                     Enabled = f.Enabled,
                     Owner = f.Owner,
                     OwnerDept = f.OwnerDept,
                     OwnerName = f.OwnerName,
                     OwnerNtid = f.OwnerNtid,
                     WhiteipList = f.WhiteipList,
                     Secrets = s.Secrets,
                     FlowNo = sc.FlowNo,
                     ApplyDescription = sc.Description?? f.Description,
                     TableId = sc.ObjectId,
                     CreateTime = sc.CreateTime
                 });//.Distinct();
            var list = await Page(allQuery, filter).ToListAsync();
            var apiList = await _cache.HashGetAllAsync<RouteInfo>(DataAssetManagerConst.RouteRedisKey);
            var hostUrl = DataAssetManagerConst.HostUrl.Replace("http://", "https://");
            list.ForEach(item =>
            {
                var apis = apiList.Where(f => f.ExecuteConfig?.tableId == item.TableId);
                item.ApiList = string.Join(",\r\n", apis.Select(f => $"[{f.ReqMethod}]{hostUrl}{f.ApiServiceUrl}").ToArray());
                item.TableName = apis.FirstOrDefault()?.ExecuteConfig?.tableName;
            });
            list = list.Distinct().ToList();
            return new PageResult<AssetClientView>(allQuery.Count(), list, filter.PageNum, filter.PageSize);
            //return await allQuery.ToListAsync();
        }


        #endregion Clients

        #region Secrets

        public async Task<List<AssetClientSecretsEntity>> AllSecrets()
        {
            return await CurrentDb.Queryable<AssetClientSecretsEntity>().ToListAsync();
        }


        public async Task<List<AssetClientSecretsEntity>> AllSecretsFromCache(bool clearCache = false)
        {
            var key = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientSecretsEntity).Name}:List";
            //if (clearCache) await _cache.RemoveAsync(key);
            return await _cache.GetObjectAsync(key, AllSecrets,null, clearCache);
        }

        public async Task<Dictionary<Guid, List<AssetClientSecretsEntity>>> AllSecretsDictFromCache(bool clearCache = false)
        {
            var key = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientSecretsEntity).Name}:Dict";
            //if (clearCache) await _cache.RemoveAsync(key);
            return await _cache.GetObjectAsync(key, async () =>
            {
                var list = await AllSecretsFromCache(clearCache);
                var data = list.GroupBy(f => f.ClientUId).Select(f =>
                   new KeyValuePair<Guid, List<AssetClientSecretsEntity>>(f.Key.Value, f.ToList()))
                   .ToDictionary();
                return data;
            }, null, clearCache);
        }

        #endregion Secrets

        #region Scopes

        public async Task<List<AssetClientScopesEntity>> AllScopes()
        {
            return await CurrentDb.Queryable<AssetClientScopesEntity>().Select(f => new AssetClientScopesEntity()
            {
                Id = f.Id,
                ClientId = f.ClientId,
                ClientName = f.ClientName,
                ClientUId = f.ClientUId,
                ObjectId = f.ObjectId,
                ObjectType = f.ObjectType,
                IsAllColumns = f.IsAllColumns,
                TableColumns = f.TableColumns,
                CtlId = f.CtlId,
                OwnerIds = f.OwnerIds,
                ConfigRule = f.ConfigRule,
                //FlowNo = f.FlowNo,
                //CreateTime = f.CreateTime,
                //Description = f.Description,
                //SearchData = f.SearchData,
            }).ToListAsync();
        }


        public async Task<List<AssetClientScopesEntity>> AllScopesFromCache(bool clearCache = false)
        {
            var key = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientScopesEntity).Name}:List";
            //if (clearCache) await _cache.RemoveAsync(key);
            return await _cache.GetObjectAsync(key, AllScopes, null, clearCache);
        }

        public async Task<Dictionary<Guid, List<AssetClientScopesEntity>>> AllScopesDictFromCache(bool clearCache = false)
        {
            var key = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientScopesEntity).Name}:Dict";
            //if (clearCache) await _cache.RemoveAsync(key);
            return await _cache.GetObjectAsync(key, async () =>
            {
                var list = await AllScopesFromCache(clearCache);
                var data = list.GroupBy(f => f.ClientUId).Select(f =>
                   new KeyValuePair<Guid, List<AssetClientScopesEntity>>(f.Key.Value, f.ToList()))
                   .ToDictionary();
                return data;
            },null, clearCache);
        }

        #endregion Scopes


        public override async Task RefreshCache<TEntity>()
        {
            await base.RefreshCache<TEntity>();
            //下面这些缓存必须要清理，否则授权信息无法刷新
            await _cache.RemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:Scope:Hash");
            await _cache.RemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(TEntity).Name}:Scope:List");
            await _cache.RemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientSecretsEntity).Name}:Dict");
            await _cache.RemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientSecretsEntity).Name}:List");
            await _cache.RemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientScopesEntity).Name}:Dict");
            await _cache.RemoveAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientScopesEntity).Name}:List");
            //await Task.Run(async () => await InitClientScopes(true));
            //await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(DataAssetManagerConst.DataTable_UserHashKey));
            await Task.CompletedTask;
        }


        public async Task<List<AssetClientDto>> AllClientScopes()
        {
            var list = await AllClientsFromCache();
            var allScopes = await AllScopesDictFromCache();
            var allSecrets = await AllSecretsDictFromCache();
            list.ForEach(item =>
            {
                if (allScopes.TryGetValue(item.Id, out var scopes))
                    item.Scopes = scopes;
                if (allSecrets.TryGetValue(item.Id, out var secrets))
                    item.Secrets = secrets;
            });
            return list;
        }

        public async Task<List<AssetClientDto>> AllClientScopesFromCache(bool clearCache = false)
        {
            if (clearCache)
            {
                await RefreshCache<AssetClientEntity>();
                //var key1 = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientScopesEntity).Name}:Dict";
                //var key2 = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientScopesEntity).Name}:List";
                //var key3 = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientSecretsEntity).Name}:Dict";
                //var key4 = $"{DataAssetManagerConst.RedisKey}{typeof(AssetClientSecretsEntity).Name}:List";
                //await _cache.DelayRemoveAsync(key1);
                //await _cache.DelayRemoveAsync(key2);
                //await _cache.DelayRemoveAsync(key3);
                //await _cache.DelayRemoveAsync(key4);
            }
            return await _cache.GetObjectAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Scope:List", AllClientScopes,null, clearCache);
        }

        /// <summary>
        /// 初始化授权信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<AssetClientDto>> InitClientScopes(bool clearCache = false)
        {
            var list = await AllClientScopesFromCache(clearCache);
            list.GroupBy(f => f.Owner).ToList().ForEach(item =>
            {
                _cache.HashSet($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Scope:Hash", item.Key, item.ToList());
            });

            //tableid拥有属于哪些user
            list.ForEach(item =>
            {
                item.Scopes.ForEach(scope =>
                {
                    var list = _cache.HashGet<List<string>>(DataAssetManagerConst.DataTable_UserHashKey, scope.ObjectId) ?? new List<string>();
                    list.Add(item.Owner);
                    list.AddRange(item.Secrets.Select(f => f.Secrets));
                    _cache.HashSet<List<string>>(DataAssetManagerConst.DataTable_UserHashKey, scope.ObjectId, data: list);
                });
            });
            return list;
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
            return list?.Any(f => f == userid) ?? false;
        }

        public async Task<List<AssetClientDto>> GetClientByClientId(string client_id)
        {
            return (await Task.FromResult(_cache.HashGet<List<AssetClientDto>>($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Scope:Hash", client_id))) ?? new List<AssetClientDto>();
            //return await _cache.GetObjectAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Scope:{client_id}", async () =>
            //{
            //    var list = await AllClientScopesFromCache();
            //    return list.Where(f => f.Owner == client_id).ToList();
            //}, TimeSpan.FromSeconds(30));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client_id"></param>
        /// <param name="clientType">1个人，2项目</param>
        /// <returns></returns>
        public async Task<List<AssetClientDto>> GetClientByClientId(string client_id, int clientType = 1)
        {
            var allSecrets = await AllClientsFromCache();
            return allSecrets.Where(f => f.ClientId == client_id && f.ClientType == clientType).ToList();
        }



        public async Task<AssetClientScopesEntity> GetClientScopesByClientId(string client_id, string tableId)
        {
            var allSecrets = await AllScopesFromCache();
            return allSecrets.Where(f => f.ClientId == client_id && f.ObjectId == tableId).FirstOrDefault();

            //var list = await Task.FromResult(_cache.HashGet<List<AssetClientDto>>($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Scope:Hash", client_id));
            //var scopes = new AssetClientScopesEntity();
            //list.ForEach(item =>
            //{
            //    var data = item.Scopes.FirstOrDefault(f => f.ObjectId == tableId);
            //    if (data != null)
            //    {
            //        scopes = data;
            //        return;
            //    }
            //});
            //return scopes;
            //return await _cache.GetObjectAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Scope:{client_id}", async () =>
            //{
            //    var list = await AllClientScopesFromCache();
            //    return list.Where(f => f.Owner == client_id).ToList();
            //}, TimeSpan.FromSeconds(30));
        }


        public async Task<AssetClientSecretsEntity> GetClientSecretsByClientId(string client_id, string secret)
        {
            var allSecrets = await AllSecretsFromCache();
            return allSecrets.Where(f => f.ClientId == client_id && f.Secrets == secret).FirstOrDefault();
        }

        //public async Task<AssetClientDto> GetClientScopesByClientId(string client_id, AuthApplyType applyType)
        //{
        //    return await _cache.GetObjectAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Scopes:{applyType}:{client_id}", async () =>
        //    {
        //        var list = await AllClientsFromCache();
        //        var allScopes = await AllScopesDictFromCache();
        //        list.ForEach(item =>
        //        {
        //            if (allScopes.TryGetValue(item.Id, out var scopes))
        //                item.Scopes = scopes;
        //        });
        //        return list.Where(f => f.Owner == client_id && f.ClientType == applyType.ToInt()).FirstOrDefault();
        //    }, TimeSpan.FromSeconds(60));
        //}


        //public async Task<AssetClientDto> GetClientSecretsByClientId(string client_id, AuthApplyType applyType)
        //{
        //    return await _cache.GetObjectAsync($"{DataAssetManagerConst.RedisKey}{typeof(AssetClientEntity).Name}:Secrets:{applyType}:{client_id}", async () =>
        //    {
        //        var list = await AllClientsFromCache();
        //        var allSecrets = await AllSecretsDictFromCache();
        //        list.ForEach(item =>
        //        {
        //            if (allSecrets.TryGetValue(item.Id, out var secrets))
        //                item.Secrets = secrets;
        //        });
        //        return list.Where(f => f.Owner == client_id && f.ClientType == applyType.ToInt()).FirstOrDefault();
        //    }, TimeSpan.FromSeconds(60));
        //}


        /// <summary>
        /// 检查指定user是否有对应权限
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public async Task<Result<List<AssetClientScopesEntity>>> CheckAuth(DataAuthCheckDto applyInfo)
        {
            var msg = new Result<List<AssetClientScopesEntity>>();
            var result = await GetAuthInfo(applyInfo);
            msg.Data = result.ownertables;
            var noOwner = result.noOwner;
            var reOwner = result.reOwner;
            var reFlow = result.reFlow;
            var onSuccess = result.onSuccess;
            if (onSuccess.Count() == 0)
            {
                StringBuilder sb = new StringBuilder();
                //if (result.isPublic.Count() > 0)
                //{
                //    sb.Append($"开放数据:{string.Join(',', result.isPublic)}");
                //    msg.SetError(sb.ToString(), 200);
                //    //sb.Append(I18nMessage.getMessage("under.applied")).append(":").append(reFlow.ToString());
                //}
                //不检查owner数据了，owner也需要申请权限
                //if (noOwner.Count() > 0)
                //{
                //    sb.Append($"No data owner:{string.Join(',', noOwner)}");
                //    msg.SetError(sb.ToString(), 402);
                //    //sb.Append(I18nMessage.getMessage("no.dataOwner")).append(":").append(noOwner.ToString());
                //}
                if (reOwner.Count() > 0)
                {
                    sb.Append($"Existing permissions:{string.Join(',', reOwner)}");
                    msg.SetError(sb.ToString(), 201);
                    //sb.Append(I18nMessage.getMessage("already.have.permission")).append(":").append(reOwner.ToString());
                }
                if (reFlow.Count() > 0)
                {
                    sb.Append($"正在审核中(Under review):{string.Join(',', reFlow)}");
                    msg.SetError(sb.ToString(), 202);
                    //sb.Append(I18nMessage.getMessage("under.applied")).append(":").append(reFlow.ToString());
                }
            }
            return msg;
        }


        // List<string> isPublic, 
        public async Task<(List<string> noOwner, List<string> reOwner, List<string> reFlow, List<string> onSuccess, List<AssetClientScopesEntity> ownertables)> GetAuthInfo(DataAuthCheckDto applyInfo)
        {
            var userId = CurrentUser.Id;// applyInfo.UserId;
            var result = new Result<string>();
            var msg = new StringBuilder();
            //从数据库获取table信息
            //_tableService.Query()

            //已有权限
            var clientid = CurrentUser.Id; //applyInfo.UserId;// applyInfo.ApplyType == AuthApplyType.Individual ? applyInfo.UserId : applyInfo.AppId ?? applyInfo.AppName;
            var ownerClient = await GetClientByClientId(clientid);
            var applyScopes = applyInfo.TableList.Select(f => f.TableId).ToList();
            var ownerTables = new List<AssetClientScopesEntity>();// ownerClient?.Scopes?.Where(f => applyScopes.Contains(f.ObjectId));
            ownerClient.ForEach(item => ownerTables.AddRange(item.Scopes.Where(f => applyScopes.Contains(f.ObjectId))));
            var ownerScopes = ownerTables.Select(f => f.ObjectId).ToList();

            //正在申请
            var recordTables = await CurrentDb.Queryable<DataAuthApplyEntity>().
                Where(f => f.UserId == userId).In("status", new string[] { "-1", "-2" })
                .Select(f => f.TableId).ToListAsync();

            var noOwner = new List<string>();
            var reOwner = new List<string>();
            var reFlow = new List<string>();
            var onSuccess = new List<string>();
            //var isPublic = new List<string>();

            foreach (var table in applyInfo.TableList)
            {
                var tableInfo = await _tableService.GetInfo(table.TableId);
                if (tableInfo == null)
                {
                    throw new Exception($"table {table.TableName} Does not exist");
                }

                var ownsId = tableInfo.OwnerList.Select(f => f.OwnerId).ToList();// table.OwnerId?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                ownsId.Remove(userId);

                //if (tableInfo.IsPublicSecurityLevel)
                //{ //没有owner
                //    isPublic.Add(table.TableName);
                //}
                //else 
                if (!tableInfo.IsPublicSecurityLevel && tableInfo.OwnerList.Count() == 0)
                { //没有owner
                    noOwner.Add(table.TableName);
                }
                //else if (ownsId.Count() == 0 || ownerScopes.Contains(table.Id))//tableInfo.IsPublicSecurityLevel || 
                //{//已有权限
                //    reOwner.Add(table.TableName);
                //}
                else if (recordTables.Contains(table.Id))
                {
                    //没有权限 但有申请
                    reFlow.Add(table.TableName);
                }
                else // if (!tableInfo.IsPublicSecurityLevel)
                {
                    onSuccess.Add(table.TableName);
                }
            }
            return await Task.FromResult((noOwner, reOwner, reFlow, onSuccess, ownerTables.ToList()));//isPublic,
        }


        /// <summary>
        /// 根据流程通过信息，更新client状态和生成token
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> CreateClientSecret(AssetClientEntity createSecret)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var entity = await CurrentDb.Queryable<AssetClientEntity>().FirstAsync(f => f.ClientId == createSecret.ClientId && f.ClientType == createSecret.ClientType);
                if (entity == null) entity = createSecret.Adapt(entity);
                await Create(entity, false);
                var secret = new AssetClientSecretsEntity()
                {
                    ClientId = entity.ClientId,
                    ClientUId = entity.Id,
                    Type = "2",
                    Secrets = MD5Util.GetMD5Value($"{createSecret.Owner}:{SnowflakeIdGenerator.NextUid()}"),
                    Description = entity.Id.ToString(),
                    CreateTime = DateTimeOffset.Now,
                };
                await Create(secret, false);
                uow.Commit();
            }
            await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(DataAssetManagerConst.DataTable_UserHashKey));
            return Result.Successd();
        }


        /// <summary>
        /// 根据流程通过信息，更新client状态和生成token
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<string>> CreateClientInfo(ApplyAuthCallbackDto createSecret)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var entity = await CurrentDb.Queryable<AssetClientEntity>().FirstAsync(f => f.ClientId == createSecret.ClientId && f.ClientType == createSecret.ClientType);

                if (entity == null)
                {
                    entity = createSecret.Adapt(entity);
                    await Create(entity, false);
                }
                else
                {
                    await ModifyHasChange(entity, false);
                }

                var secret = new AssetClientSecretsEntity()
                {
                    ClientId = entity.ClientId,
                    ClientUId = entity.Id,
                    FlowNo = createSecret.FlowNo,
                    OwnerId = createSecret.Owner,
                    Type = createSecret.ClientType.ToString(),
                    Secrets = MD5Util.GetMD5Value($"{createSecret.Owner}:{SnowflakeIdGenerator.NextUid()}"),
                    Description = entity.Id.ToString(),
                    CreateTime = DateTimeOffset.Now,
                };
                //一个client，只能有一个secert
                if (!CurrentDb.Queryable<AssetClientSecretsEntity>().Any(f => f.ClientId == createSecret.ClientId && f.Type == ((int)createSecret.ClientType).ToString()))
                    await Create(secret, false);

                if (!(createSecret.FlowDetails == null || createSecret.FlowDetails.Count == 0))
                {
                    var apiList = await _cache.HashGetAllAsync<RouteInfo>(DataAssetManagerConst.RouteRedisKey);
                    foreach (var item in createSecret.FlowDetails)
                    {
                        var scope = new AssetClientScopesEntity()
                        {
                            ClientId = entity.ClientId,
                            ClientUId = entity.Id,
                            ClientName = entity.ClientName,
                            CtlId = item.CtlId,
                            IsAllColumns = item.AllColumns,
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType,
                            FlowNo = createSecret.FlowNo,
                            Description = createSecret.Description,
                            TableColumns = item.TableColumns.Adapt<List<DataColumnDto>>(),
                            SearchData = $"{entity.ClientName};{item.ObjectName};{string.Join(";", apiList.Where(f => f.TableId == item.ObjectId).Select(f => f.ApiUrl))};{string.Join(";",item.TableColumns.Select(f=>f.ColName))}".ToLower(),
                            OwnerIds = createSecret.TableOwner?.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries),
                        };
                        await Create(scope, false);
                    }
                }

                uow.Commit();

                await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(DataAssetManagerConst.DataTable_UserHashKey));
                return Result.Successd(secret.Secrets);
            }
        }

        public async Task<List<AssetClientDto>> GetApplictionList(string appName, string userid)
        {
            return (await CurrentDb.Queryable<AssetClientEntity>().Where(f => f.ClientType == 2)
                .WhereIF(!appName.IsNullOrWhiteSpace(), f => f.ClientName.Contains(appName))
                .WhereIF(!userid.IsNullOrWhiteSpace(), f => f.Owner.Contains(userid) || SqlFunc.JsonListObjectAny(f.SMEList, "UserId", userid))
                .OrderByDescending(f => f.CreateTime).ToListAsync()).Adapt<List<AssetClientDto>>();
        }


        /// <summary>
        /// 添加权限
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(true)]
        public async Task<IResult> UpdataClientScopes(AuthUserTableDto input)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (input.UserList == null || input.Node == null) throw new Exception($"Parameter abnormality！");
                Result<List<UserInfo>> userList = null;
                foreach (var item in input.UserList)
                {
                    if (input.Type.Equals("update", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = await CurrentDb.Deleteable<AssetClientScopesEntity>().Where(it => it.ClientId.Equals(item.Id)).ExecuteCommandAsync();
                    }
                    var entity = await CurrentDb.Queryable<AssetClientEntity>().FirstAsync(f => f.ClientId == item.Id && f.ClientType == 1);
                    if (entity == null)
                    {
                        var userInfo = new UserInfo() { UserId = item.Id, UserName = item.Username };
                        if (userList == null) userList = await _userProxyService.GetUsersAsync();
                        if (userList.Success) userInfo = userList.Data.FirstOrDefault(f => f.UserId.Equals(item.Id, StringComparison.CurrentCultureIgnoreCase));
                        if (userInfo == null) userInfo = new UserInfo() { UserId = item.Id, UserName = item.Username };
                        entity = new AssetClientEntity()
                        {
                            ClientId = item.Id,
                            ClientName = item.Username,
                            ClientType = 1,
                            Owner = item.Id,
                            OwnerName = item.Username,
                            OwnerNtid = userInfo.NtId,
                            OwnerDept = userInfo.Department,
                            Description = "授权添加",
                            Enabled = true
                        };
                        await Create(entity, false);
                    }
                    else
                    {
                        if (entity.OwnerDept.IsNullOrWhiteSpace())
                        {
                            var userInfo = new UserInfo() { UserId = item.Id, UserName = item.Username };
                            if (userList == null) userList = await _userProxyService.GetUsersAsync();
                            if (userList.Success) userInfo = userList.Data.FirstOrDefault(f => f.UserId.Equals(item.Id, StringComparison.CurrentCultureIgnoreCase));
                            if (userInfo != null)
                            {
                                entity.OwnerName = item.Username;
                                entity.OwnerNtid = userInfo.NtId;
                                entity.OwnerDept = userInfo.Department;
                                entity.Description = "授权添加";
                            }
                        }
                        await ModifyHasChange(entity, false);
                    }

                    if (!(input.Node == null || input.Node.Count == 0))
                    {
                        foreach (var table in input.Node)
                        {
                            var data = await AsQueryable<AssetClientScopesEntity>().FirstAsync(f => f.ObjectType == table.Type
                            && f.ClientId == entity.ClientId
                            && f.ObjectId == table.Key);
                            if (data == null)
                            {
                                var scope = new AssetClientScopesEntity()
                                {
                                    ClientId = entity.ClientId,
                                    ClientUId = entity.Id,
                                    ClientName = entity.ClientName,
                                    CtlId = table.PId,
                                    IsAllColumns = true,
                                    ObjectId = table.Key,
                                    ObjectType = table.Type,
                                    TableColumns = new List<DataColumnDto>(),
                                    OwnerIds = new string[0],
                                };
                                await Create(scope, false);
                            }
                            else
                            {
                                data.IsAllColumns = true;
                                data.ObjectId = table.Key;
                                data.ObjectType = table.Type;
                                data.TableColumns = new List<DataColumnDto>();
                                data.OwnerIds = new string[0];
                                await ModifyHasChange(data, false);
                            }
                        }
                    }
                }
                uow.Commit();
            }
            //await RefreshCache();
            return Result.Successd();
        }


        public async Task<Result<List<UserInfo>>?> GetUsersAsync()
        {
          return  await _userProxyService.GetUsersAsync();
        }


        public override async Task<int> Create<TEntity>(TEntity entity, bool clearCache = true)
        {
            if (entity is AssetClientEntity)
            {
                var data = entity as AssetClientEntity;
                if (data.ClientId.IsNullOrWhiteSpace()) data.ClientId = Guid.NewGuid().ToString("N");
                return await base.Create(data, clearCache);
            }
            else
                return await base.Create(entity, clearCache);
        }
        public override async Task<int> Modify<TEntity>(TEntity entity, bool clearCache = true)
        {
            if (entity is AssetClientEntity)
            {
                var model = entity as AssetClientEntity;
                var data = await Get(entity.Id);
                if (data.ClientId.IsNotNullOrWhiteSpace() && !data.ClientId.Equals(model.ClientId) && CurrentDb.Queryable<AssetClientSecretsEntity>().Any(f => f.ClientUId == entity.Id))
                {
                    throw new AppFriendlyException("The current app has been used, update ClientId! 当前app已经被使用，更新ClientId!", 8002);
                }
            }
            return await base.Modify(entity, clearCache);
        }

        public override async Task<bool> Delete(Guid id, bool clearCache = true)
        {
            if (CurrentDb.Queryable<AssetClientSecretsEntity>().Any(f => f.ClientUId == id))
            {
                throw new AppFriendlyException("The current app has been used and cannot be deleted! 当前app已经被使用，不可以删除!", 8001);
            }
            return await base.Delete(id, clearCache);
        }
    }
}
