using DataAssetManager.DataApiServer.Application.DataChangeRecord.Dtos;
using DataAssetManager.DataApiServer.Core;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataChangeRecordService : BaseService<DataChangeRecordEntity, DataChangeRecordDto, string>, IDataChangeRecordService, ITransient
    {
        public DataChangeRecordService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache)
        {
        }

        public override ISugarQueryable<DataChangeRecordEntity> BuildFilterQuery(DataChangeRecordDto filter)
        {
            return CurrentDb.Queryable<DataChangeRecordEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectId), f => f.ObjectId == filter.ObjectId)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectType), f => f.ObjectType == filter.ObjectType)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.FieldName), f =>SqlFunc.ToLower( f.FieldName).Contains(filter.FieldName.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.FieldNewValue), f => SqlFunc.ToLower(f.FieldNewValue).Contains(filter.FieldNewValue.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.FieldOldValue), f => SqlFunc.ToLower(f.FieldOldValue).Contains(filter.FieldOldValue.ToLower()))
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .WhereIF(filter.Version.HasValue, f => f.Version == filter.Version)
                .OrderBy(f => f.Id);
        }


        public async Task<int> SaveBatch(List<DataChangeRecordEntity> columns)
        {
            return await CurrentDb.Storageable(columns).ExecuteSqlBulkCopyAsync();
        }



        public async Task<List<DataChangeRecordEntity>> GetByTableId(string objectType)
        {
            return await CurrentDb.Queryable<DataChangeRecordEntity>().Where(f => f.ObjectType == objectType).ToListAsync();
        }


    }
}
