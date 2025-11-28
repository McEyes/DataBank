using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application
{
    public class ApiFeedbackService : BaseService<ApiFeedbackEntity, ApiFeedbackDto, Guid>, IApiFeedbackService, ITransient
    {

        public ApiFeedbackService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache)
        {
        }

        public override ISugarQueryable<ApiFeedbackEntity> BuildFilterQuery(ApiFeedbackDto filter)
        {
            return CurrentDb.Queryable<ApiFeedbackEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserId), f =>SqlFunc.ToLower( f.UserId) == filter.UserId.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectId), f => f.ObjectId == filter.ObjectId)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserName), f => SqlFunc.ToLower(f.UserName).Contains(filter.UserName.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ObjectName), f => f.ObjectName.Contains(filter.ObjectName))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.OwnerId), f => SqlFunc.ToLower(f.OwnerId).Contains(filter.OwnerId.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserDept), f => SqlFunc.ToLower(f.UserDept).Contains(filter.UserDept.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Status), f => f.Status == filter.Status)
                .OrderByDescending(f => f.CreateTime);
        }


    }
}
