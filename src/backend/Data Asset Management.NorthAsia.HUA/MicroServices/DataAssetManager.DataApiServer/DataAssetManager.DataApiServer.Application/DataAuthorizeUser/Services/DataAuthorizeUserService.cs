using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch.MachineLearning;

using Furion.DatabaseAccessor;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using StackExchange.Profiling.Internal;

using System.Linq;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataAuthorizeUserService : BaseService<DataAuthorizeUserEntity, DataAuthorizeUserDto, string>, IDataAuthorizeUserService, ITransient
    {
        private readonly IAssetClientsService _assetClientsService;

        public DataAuthorizeUserService(ISqlSugarClient db, IDistributedCacheService cache, IAssetClientsService assetClientsService) : base(db, cache, true, true)
        {
            _assetClientsService = assetClientsService;
        }

        public override ISugarQueryable<DataAuthorizeUserEntity> BuildFilterQuery(DataAuthorizeUserDto filter)
        {
            return CurrentDb.Queryable<DataAuthorizeUserEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserId), f => SqlFunc.ToLower(f.UserId) == filter.UserId.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserName), f => SqlFunc.ToLower(f.UserId) == filter.UserName.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectId), f => f.ObjectId.Equals(filter.ObjectId));
            //.WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectName), f => f.ObjectName == filter.ObjectName)
        }

        [UnitOfWork(true)]
        public async Task SaveAuth(AuthUserTableDto input)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (input.UserList == null || input.Node == null) throw new Exception($"²ÎÊýÒì³££¡");
                input.Node = input.Node.Where(f => f.Type == "table").ToList();
                foreach (var item in input.UserList)
                {
                    if (input.Type.Equals("update", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = await CurrentDb.Deleteable<DataAuthorizeUserEntity>().Where(it => it.UserId.Equals(item.Id)).ExecuteCommandAsync();
                    }
                    var list = new List<DataAuthorizeUserEntity>();
                    foreach (var node in input.Node)
                    {
                        list.Add(new DataAuthorizeUserEntity
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = item.Id,
                            UserName = item.Username,
                            ObjectId = node.Key,
                            ObjectType = node.Type,
                            CtlId = node.PId,
                        });
                    }
                    await CurrentDb.Storageable(list).ExecuteCommandAsync();//.ExecuteSqlBulkCopyAsync();
                }
                var clientCesult = await _assetClientsService.UpdataClientScopes(input);
                if (clientCesult.Success) uow.Commit();
            }
            await SendEvent(DataAssetManagerConst.DataAuthRefreshEvent);
            await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Create", input, typeof(AuthUserTableDto));
        }

        public async Task<PageResult<DataAuthUserTableDto>> UserAuthList(PageEntity<string> filter)
        {
            var userid = CurrentUser?.UserId ?? "";
            if (CurrentUser.IsDataAssetManager) userid = string.Empty;
            filter.Keyword = filter.Keyword?.Trim().ToLower();
            //if (string.IsNullOrWhiteSpace(userid)) return new PageResult<DataAuthUserTableDto>();
            var query = CurrentDb.Queryable<DataAuthorizeUserEntity>()
                           .LeftJoin<DataTableEntity>((dau, dt) => dau.ObjectId == dt.Id)
                           .WhereIF(!string.IsNullOrWhiteSpace(userid), (dau, dt) => dau.UserId.Equals(userid))
                           .WhereIF(!filter.Keyword.IsNullOrWhiteSpace(), (dau, dt) =>
                                 SqlFunc.ToLower(dt.TableComment).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.TableName).Contains(filter.Keyword) ||
                                 SqlFunc.ToLower(dt.Alias).Contains(filter.Keyword)
                                 || SqlFunc.ToLower(dau.UserName).Contains(filter.Keyword))
                           .GroupBy((dau, dt) => new
                           {
                               dau.UserId,
                               dau.UserName
                           })
                           .Select((dau, dt) =>
                            new DataAuthUserTableDto
                            {
                                UserId = dau.UserId,
                                UserName = dau.UserName,
                                TableName = SqlFunc.MappingColumn(default(string), "STRING_AGG(DISTINCT dt.table_name,',')")
                            });
            var list = await query.Skip(filter.SkipCount).Take(filter.PageSize).ToListAsync();
            return new PageResult<DataAuthUserTableDto>(query.Count(), list,
                filter.PageNum, filter.PageSize);
        }

        [UnitOfWork(true)]
        public async Task<int> DeleteByUser(string userid)
        {
            var result = 0;
            using (var uow = CurrentDb.CreateContext())
            {
                result = await CurrentDb.Deleteable<DataAuthorizeUserEntity>().Where(f => f.UserId == userid).ExecuteCommandAsync();
                if (result > 0)
                {
                    var list = await _assetClientsService.GetClientByClientId(userid, 1);
                    var uidList = list.Select(f => f.Id).ToList();
                    result = await CurrentDb.Deleteable<AssetClientScopesEntity>().Where(f => uidList.Contains(f.ClientUId.Value)).ExecuteCommandAsync();
                }
                if (result > 0)
                {
                    uow.Commit();
                    await SendEvent(DataAssetManagerConst.DataAuthRefreshEvent);
                    await SendLogEvent($"{DataAssetManagerConst.RedisKey}DB:Delete", userid, typeof(string));
                }
            }
            return result;
        }
    }
}
