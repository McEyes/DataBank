using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using MetadataManagement.Core.Entitys;

namespace MetadataManagement.Application
{
    public class DataTableService : BaseService<DataTableEntity, DataTableQuery, long>, IDataTableService, ITransient
    {
        public DataTableService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache, false, false, true)
        {

        }

        public override ISugarQueryable<DataTableEntity> BuildFilterQuery(DataTableQuery filter)
        {
            filter.Keyword = filter.Keyword?.Trim()?.ToLower();
            return base.CurrentDb.Queryable<DataTableEntity>()
                .WhereIF(filter.Keyword.IsNotNullOrWhiteSpace(), f => f.Name.Contains(filter.Keyword) || f.Code.Contains(filter.Keyword) || f.EnglishName.Contains(filter.Keyword))
                .WhereIF(filter.Code.IsNotNullOrWhiteSpace(), f => f.Code.Equals(filter.Code.Trim().ToLower()))
                .WhereIF(filter.Name.IsNotNullOrWhiteSpace(), f => f.Name.Equals(filter.Name.Trim().ToLower()));
                //.WhereIF(filter.Status.HasValue, f => f.en.Equals(filter.Status));
        }
    }
}
