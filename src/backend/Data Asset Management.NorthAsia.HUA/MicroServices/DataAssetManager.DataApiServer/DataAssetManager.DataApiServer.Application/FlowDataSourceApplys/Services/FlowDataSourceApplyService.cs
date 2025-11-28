using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataApiServer.Application.FlowDataSourceApply.Services;
using DataAssetManager.DataApiServer.Application.FlowDataSourceApplys.Dtos;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch.MachineLearning;

using Furion.DatabaseAccessor;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using static Grpc.Core.Metadata;
using System.Globalization;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using Mapster;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;

namespace DataAssetManager.DataApiServer.Application
{
    public class FlowDataSourceApplyService : BaseService<FlowDataSourceApplyEntity, FlowDataSourceApplyDto, Guid>, IFlowDataSourceApplyService, ITransient
    {
        private readonly IDataAuthorizeUserService _authorizeUserService;
        //private readonly string HomeUrl;
        //private readonly string WorkflowInitUrl = "/homeapi/FlowDataGrantApply/InitFlow";

        //private readonly IUserService _userService;
        private readonly IEmployeeBaseInfoService _employeeService;
        private readonly IDataColumnService _columnService;
        private readonly IDataUserService _dataUserService;
        private readonly FlowApplyProxyService _flowApplyProxyService;
        private readonly ILogger<FlowDataSourceApplyService> _logger;

        public FlowDataSourceApplyService(ISqlSugarClient db, IDistributedCacheService cache,
             IEmployeeBaseInfoService employeeService,
             DataColumnService columnService,
             IDataUserService dataUserService,
             FlowApplyProxyService flowApplyProxyService,
             ILogger<FlowDataSourceApplyService> logger,
            IDataAuthorizeUserService authorizeUserService) : base(db, cache)
        {
            _authorizeUserService = authorizeUserService;
            //HomeUrl = App.GetConfig<string>("RemoteApi:HomeUrl");
            //var flowInitUrl = App.GetConfig<string>("RemoteApi:WorkflowInitUrl");
            //if (!flowInitUrl.IsNullOrWhiteSpace()) WorkflowInitUrl = flowInitUrl;
            _employeeService = employeeService;
            _columnService = columnService;
            _dataUserService = dataUserService;
            _flowApplyProxyService = flowApplyProxyService;
            _logger = logger;
        }

        public override ISugarQueryable<FlowDataSourceApplyEntity> BuildFilterQuery(FlowDataSourceApplyDto filter)
        {
            return CurrentDb.Queryable<FlowDataSourceApplyEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.FormNo), f =>SqlFunc.ToLower( f.FormNo) == filter.FormNo.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserId), f => SqlFunc.ToLower(f.UserId) == filter.UserId.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserName), f => SqlFunc.ToLower(f.UserName) == filter.UserName.ToLower())
                .OrderByDescending(f => f.CreateTime);
        }


        [UnitOfWork(true)]
        public async Task<ITPortal.Core.Services.IResult> ApplyAuth(FlowSourceApply applyData)
        {
            var entity = applyData.Adapt<FlowDataSourceApplyEntity>();
            var userId = applyData.UserId;
            var userName = applyData.UserName;
            entity.Id = Guid.NewGuid();
            entity.UserId = userId;  //申请人Id
            entity.UpdateBy = CurrentUser?.UserName ?? "test";
            //if (applyData.SmeList != null && applyData.SmeList.Count > 0)
            //{
            //    entity.SMEUserId = applyData.SmeList[0];
            //    //entity.SMEUserName = applyData.SmeList[0].Name;
            //}

            if (string.IsNullOrEmpty(applyData.Superior))
            {
                throw new AppFriendlyException("Superior is empty", "5001");
            }
            if (string.IsNullOrEmpty(entity.SMEUserId))
            {
                throw new AppFriendlyException("SME Approver is empty", "5001");
            }
            if (string.IsNullOrEmpty(entity.BASUserId))
            {
                throw new AppFriendlyException("BAS Approver is empty", "5001");
            }
            //if (string.IsNullOrEmpty(applyData.Approver))
            //{
            //    throw new AppFriendlyException("IT Approver is empty", "5001");
            //}


            using (var uow = CurrentDb.CreateContext())
            {
                var result = await StartFlow(entity);
                if (result.Success)
                {
                    entity.Status = result.Success ? 1 : -2;
                    await Create(entity);
                    uow.Commit();
                }
                return result;
            }
        }


        [UnitOfWork(true)]
        public async Task<Result<FlowInstEntity>> StartFlow(FlowDataSourceApplyEntity entity)
        {
            StartFlowDto flwoData = new StartFlowDto();

            var tempInfoResult = await _flowApplyProxyService.GetFlowTempInfo("ITPortal_LakeEntryApplication");
            if (!tempInfoResult.Success) return new Result<FlowInstEntity>().SetError(tempInfoResult.Msg.ToString());
            var tempInfo = tempInfoResult.Data;

            //查询申请人上级id --需要上级主管审批
            JabusEmployeeInfo manage = await _employeeService.GetManagerAsync(entity.UserId);
            await SetActivelActor("Superior Approval", manage.WorkNTID, tempInfo);

            //SME审批
            await SetActivelActor("SME Approval", entity.SMEUserId, tempInfo);

            //BSA审批
            await SetActivelActor("BSA Approval", entity.BASUserId, tempInfo);

            //申请人确认
            await SetActivelActor("Entry Demand Confirmation", entity.UserId, tempInfo);

            //数据入湖
            await SetActivelActor("Data Link", entity.BASUserId, tempInfo);

            //数据验证
            await SetActivelActor("Data Verify", entity.SMEUserId, tempInfo);

            //IT审批人
            //await SetActivelActor("IT Approval", entity.Approver, tempInfo);

            var result = await _flowApplyProxyService.InitFlowAsync(new StartFlowDto()
            {
                FlowTempId = tempInfo.Id.ToString(),
                FormData = entity,
                FormId = entity.Id,
                Applicant = entity.UserId,
                ApplicantName = entity.UserName,
                ActApprovers = tempInfo.FlowActs.Adapt<List<FlowActApprover>>(),
                Attacchments = entity.FileList
            });
            return result;
        }



        /// <summary>
        /// 审批回调
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [UnitOfWork(true)]
        public async Task<ITPortal.Core.Services.IResult<string>> CallBack(Result<FlowBackDataEntity> result)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                if (result.Data == null || result.Data.FlowInst == null) throw new AppFriendlyException("FlowDataSourceApply CallBack error,no call back data param", "5201");
                var entity = await CurrentDb.Queryable<FlowDataSourceApplyEntity>().FirstAsync(f => f.Id == result.Data.FlowInst.Id);
                if (entity == null) return await Task.FromResult(new Result<string>() { Data = "" });
                else if (result.Success)
                {
                    entity.Status = (int)result.Data.FlowInst.FlowStatus;
                }
                else
                {
                    entity.Status = (int)ITPortal.Core.ProxyApi.Flow.Enums.FlowStatus.Stop;
                }
                await ModifyHasChange(entity);
                uow.Commit();
            }
            return await Task.FromResult(new Result<string>() { Data = "" });
        }

        private async Task SetActivelActor(string actName, string userid, FlowTemplateDto tempInfo)
        {
            var nextApproval = tempInfo.FlowActs.FirstOrDefault(f => f.ActName == actName);
            if (nextApproval == null) throw new AppFriendlyException($"审批节点“{actName}”名称错误，审批人配置失败", "5004");
            if (nextApproval.IsLockApprover == true && nextApproval.ActorParms.Count > 0)
                return;
            var approvor = await _employeeService.GetEmployeeInfo(userid);
            if (approvor == null) throw new AppFriendlyException($"审批节点“{actName}”审批人配置失败,{userid}审批人不存在", "5004");
            nextApproval.ActorParms.Clear();
            nextApproval.ActorParms.Add(approvor.Adapt<StaffInfo>());
            nextApproval.ActorParmsName = approvor.Name;
        }

        /// <summary>
        /// 根据审批结果 输入入库
        /// </summary>
        /// <param name="authResultDto"></param>
        /// <returns></returns>
        [UnitOfWork(true)]
        public async Task<ITPortal.Core.Services.IResult<string>> UpdateAuth(AuthResultDto authResultDto)
        {
            return await Task.FromResult(new Result<string>() { Data = "" });
        }

    }
}
