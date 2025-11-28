using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Flow.Application.FlowTodo.Dtos;

namespace ITPortal.Flow.Application.FlowTodo.Services
{
    public class FlowTodoService : BaseService<FlowTodoEntity, FlowTodoDto, Guid>, IFlowTodoService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowTodoService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowTodoEntity> BuildFilterQuery(FlowTodoDto filter)
        {
            return CurrentDb.Queryable<FlowTodoEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(filter.Title.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Title).Contains(filter.Title.ToLower()))
                .WhereIF(filter.OwnerID.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.OwnerID).Equals(filter.OwnerID.ToLower()))
                .WhereIF(filter.Applicant.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Applicant).Contains(filter.Applicant.ToLower()))
                .WhereIF(filter.ApplicantName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.ApplicantName).Contains(filter.ApplicantName.ToLower()))
                .WhereIF(filter.Approver.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Approver).Equals(filter.Approver.ToLower()))
                .WhereIF(filter.ApproverName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.ApproverName).Contains(filter.ApproverName.ToLower()))
                .WhereIF(filter.FlowStatus.HasValue, f => f.FlowStatus == filter.FlowStatus)
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .WhereIF(filter.StartTime.HasValue, f => f.CreateTime >= filter.StartTime)
                .WhereIF(filter.EndTime.HasValue, f => f.CreateTime >= filter.EndTime)
                .OrderByDescending(f=>f.CreateTime);
        }

        public virtual async Task<PageResult<FlowTodoEntity>> PageSelfTodo(FlowTodoDto filter)
        {
            filter.OwnerID = CurrentUser.Id;
            filter.FlowStatus = Core.FlowStatus.Running;
            filter.Status = Core.TodoStatus.Todo;
            return await PageQuery(filter);
        }


        public virtual async Task<PageResult<FlowTodoEntity>> PageDealTodo(FlowTodoDto filter)
        {
            filter.OwnerID = CurrentUser.Id;
            filter.Status = null;
            var query = BuildFilterQuery(filter);
            query.Where(f => f.Status != Core.TodoStatus.Todo);
            return new PageResult<FlowTodoEntity>(query.Count(), await Paging(query, filter).ToListAsync(), filter.PageNum, filter.PageSize);
        }

    }
}
