using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application
{
    public class AssetDocService : BaseService<AssetDocEntity, AssetDocDto, Guid>, IAssetDocService, ITransient
    {

        public AssetDocService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache)
        {
        }

        public override ISugarQueryable<AssetDocEntity> BuildFilterQuery(AssetDocDto filter)
        {
            return CurrentDb.Queryable<AssetDocEntity>()
                .WhereIF(filter.Id.HasValue, f => f.Id == filter.Id)
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Name), f => SqlFunc.ToLower(f.Name).Contains(filter.Name.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Catalog), f => SqlFunc.ToLower(f.Catalog) == filter.Catalog.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Keyword), f => SqlFunc.ToLower(f.Name).Contains(filter.Keyword.ToLower()) || SqlFunc.ToLower(f.DisplayNameCn).Contains(filter.Keyword.ToLower()) || SqlFunc.ToLower(f.DisplayNameEn).Contains(filter.Keyword.ToLower()))
                .OrderBy(f => f.Sort);
        }

        public override async Task<int> Modify<TEntity>(TEntity entity, bool clearCache = true)
        {
            var model = await Get<TEntity>(entity.Id);
            if (model is AssetDocEntity)
            {
                var oldModel = (model as AssetDocEntity);
                var newEntity = (entity as AssetDocEntity);
                if (newEntity.Url != oldModel.Url) newEntity.DocVer = oldModel.DocVer + 1;
            }
            return await base.Modify(entity, clearCache);
        }
    }
}
