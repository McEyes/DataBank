using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataColumnService : BaseService<DataColumnEntity, DataColumnDto, string>, IDataColumnService, ITransient
    {
        public DataColumnService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache,false,false,true)
        {
        }

        public override ISugarQueryable<DataColumnEntity> BuildFilterQuery(DataColumnDto filter)
        {
            return CurrentDb.Queryable<DataColumnEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.TableId), f => f.TableId == filter.TableId)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.SourceId), f => f.SourceId == filter.SourceId)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ColName), f => SqlFunc.ToLower(f.ColName) == filter.ColName.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ColKey), f => SqlFunc.ToLower(f.ColKey) == filter.ColKey.ToLower())
                .OrderBy("column_position,column_name");
        }

        //public async Task<List<DataColumnEntity>> All()
        //{
        //    //查询表的所有
        //    var list = await CurrentDb.Queryable<DataColumnEntity>().ToListAsync();
        //    return list;
        //}

        //public async Task<List<DataColumnEntity>> AllFromCache()
        //{
        //    return await _cache.HashGetAllAsync(DataAssetManagerConst.DataColumn_HashKey, async () =>
        //    {
        //        var list = await All();
        //        foreach (var item in list)
        //        {
        //            _cache.HashSet(DataAssetManagerConst.DataColumn_HashKey, item.Id, item);
        //        }
        //        return list;
        //    });
        //}

        public async Task<int> SaveBatch(List<DataColumnEntity> columns)
        {
            return await CurrentDb.Storageable(columns).ExecuteSqlBulkCopyAsync();
        }



        public async Task<List<DataColumnEntity>> GetByTableId(string tableId)
        {
            return await CurrentDb.Queryable<DataColumnEntity>().Where(f => f.TableId == tableId).OrderBy("column_position,column_name").ToListAsync();
        }

        public async Task<int> DeleteByTableId(string tableId)
        {
            return await CurrentDb.Deleteable<DataColumnEntity>().Where(f => f.TableId == tableId).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
        }
        public async Task<int> DeleteByTableId(dynamic[] tableIds)
        {
            return await CurrentDb.Deleteable<DataColumnEntity>().Where(f => tableIds.Contains(f.TableId)).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
        }

    }
}
