using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataUserService : BaseService<MetaDataUserEntity, MetaDataUserDto, string>, IDataUserService, ITransient
    {

        public DataUserService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache)
        {
        }

        public override ISugarQueryable<MetaDataUserEntity> BuildFilterQuery(MetaDataUserDto filter)
        {
            return CurrentDb.Queryable<MetaDataUserEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserId), f => SqlFunc.ToLower(f.UserId) == filter.UserId.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserName), f => SqlFunc.ToLower(f.UserId) == filter.UserName.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectType), f => f.ObjectType == filter.ObjectType)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectId), f => f.ObjectId.Equals(filter.ObjectId));
        }

        public async Task<List<ApproveDto>> GetTableApprovers(string tableId)
        {
            var list = await CurrentDb.Queryable<MetaDataUserEntity>()
                   .Where(f => f.ObjectId == tableId && f.ObjectType == "table")
                   .GroupBy(f => new { f.Sort })
                   .Select(g => new
                   {
                       Sort = g.Sort,
                       UserList = SqlFunc.Subqueryable<MetaDataUserEntity>()
                       .Where(s => s.Sort == g.Sort && s.ObjectId == tableId && s.ObjectType == "table").ToList(),
                   })
                   .ToListAsync();
            var result = new List<ApproveDto>();
            foreach (var item in list)
                result.Add(new ApproveDto() { Sort = item.Sort, UserList = item.UserList.Adapt<List<ApproveUser>>() });
            return result;
        }

        public async Task<List<MetaDataUserEntity>> GetDataUserByTableId(string tableId)
        {
            return await CurrentDb.Queryable<MetaDataUserEntity>()
                   .Where(f => f.ObjectId == tableId && f.ObjectType == "table")
                   .ToListAsync();
        }

        public async Task<int> DeleteByTableId(string tableId)
        {
            return await CurrentDb.Deleteable<MetaDataUserEntity>().EnableDiffLogEventIF(EnableDiffLogEvent)
                   .Where(f => f.ObjectId == tableId && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent)
                   .ExecuteCommandAsync();
        }
        public async Task<int> DeleteByTableId(dynamic[] tableId)
        {
            return await CurrentDb.Deleteable<MetaDataUserEntity>().EnableDiffLogEventIF(EnableDiffLogEvent)
                   .Where(f => tableId.Contains(f.ObjectId) && f.ObjectType == "table").EnableDiffLogEventIF(EnableDiffLogEvent)
                   .ExecuteCommandAsync();
        }
    }
}
