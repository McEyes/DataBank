using Elastic.Clients.Elasticsearch.IndexManagement;

using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Flow.Application.Common;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;
using ITPortal.Flow.Application.FlowTemplate.Dtos;

using static Grpc.Core.Metadata;

namespace ITPortal.Flow.Application.FlowTemplate.Services
{
    public class FlowTemplateService : BaseService<FlowTemplateEntity, FlowTemplateDto, Guid>, IFlowTemplateService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowTemplateService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowTemplateEntity> BuildFilterQuery(FlowTemplateDto filter)
        {
            return CurrentDb.Queryable<FlowTemplateEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(filter.FlowName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowName).Equals(filter.FlowName.ToLower()))
                .WhereIF(filter.FlowTitle.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowTitle).Contains(filter.FlowTitle.ToLower()))
                .WhereIF(filter.Status.HasValue, f => f.Status.Equals(filter.Status));
        }

        public async Task<FlowTemplateDto> GetTempInfo(string tempName)
        {
            var tempData = await CurrentDb.Queryable<FlowTemplateEntity>().OrderByDescending(f=>f.UpdateTime).FirstAsync(f => f.Status.Equals(1) && SqlFunc.ToLower(f.FlowName).Equals(tempName.ToLower()));
            return await GetTempInfo(tempData);
        }

        public async Task<FlowTemplateDto> GetTempInfo(Guid tempId)
        {
            return await GetTempInfo( await Get(tempId));
        }

        private async Task<FlowTemplateDto> GetTempInfo(FlowTemplateEntity entity)
        {
            if(entity==null)return null;
            var model = entity.Adapt<FlowTemplateDto>();
            var actList = await CurrentDb.Queryable<FlowTempActEntity>().Where(f => f.FlowTempID == model.Id).ToListAsync();
            var actRouteList = await CurrentDb.Queryable<FlowTempActRouteEntity>().Where(f => f.FlowTempID == model.Id).ToListAsync();
            foreach (var act in actList)
            {
                var actModel = act.Adapt<FlowTempActDto>();
                actModel.SwitchPath = actRouteList.Where(f => f.ActID == act.Id).ToList().Adapt<List<FlowTempActRouteDto>>();
                model.FlowActs.Add(actModel);
            }
            return model;
        }

        /// <summary>
        /// 检查模板路由情况
        /// </summary>
        /// <param name="tempId"></param>
        /// <returns></returns>
        public async Task<Result<string>> CheckTempRoute(Guid tempId)
        {
            var result = new Result<string>();
            var tempInfo = await GetTempInfo(tempId);
            var noRouteAct = tempInfo.FlowActs.Where(f => f.SwitchPath.Count == 0 && f.ActName != "End").ToList();
            foreach (var item in noRouteAct)
            {
                result.SetError($"{item.ActTitle}节点路由异常，没有匹配到下一个节点信息：{string.Join(",", item.SwitchPath.Select(f => f.NextActInsName))}");
            }
            if (!result.Success)
            {
                result.Msg = $"以下审批节点路由信息异常：{result.Msg}";
            }
            return result;
        }



    }
}
