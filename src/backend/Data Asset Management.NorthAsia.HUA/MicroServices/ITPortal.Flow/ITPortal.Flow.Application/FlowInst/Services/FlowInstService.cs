using Elastic.Clients.Elasticsearch;

using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Flow.Application.Common;
using ITPortal.Flow.Application.EmailSendRecord.Dtos;
using ITPortal.Flow.Application.EmailTemplate.Dtos;
using ITPortal.Flow.Application.FlowActInst.Dtos;
using ITPortal.Flow.Application.FlowAttachments.Dtos;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowInst.Dtos;
using ITPortal.Flow.Application.FlowInstActRoute.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Application.FlowTemplate.Dtos;
using ITPortal.Flow.Application.FlowTemplate.Services;
using ITPortal.Flow.Application.FlowTodo.Dtos;
using ITPortal.Flow.Core;
using ITPortal.Flow.Core.Enums;

using Mapster;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using Npgsql.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using IResult = ITPortal.Core.Services.IResult;
using Result = ITPortal.Core.Services.Result;

namespace ITPortal.Flow.Application.FlowInst.Services
{
    public class FlowInstService : BaseService<FlowInstEntity, FlowInstDto, Guid>, IFlowInstService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        private readonly MyEmployeeProxyService _employeeProxyService;
        private readonly IFlowTemplateService _flowTemplateService;
        private readonly ILogger<FlowInstService> _logger;
        public FlowInstService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache, MyEmployeeProxyService employeeProxyService
            , IFlowTemplateService flowTemplateService
            , ILogger<FlowInstService> logger)
            : base(db, cache, true, false)
        {
            _eventPublisher = eventPublisher;
            _employeeProxyService = employeeProxyService;
            _flowTemplateService = flowTemplateService;
            _logger = logger;
        }

        public override ISugarQueryable<FlowInstEntity> BuildFilterQuery(FlowInstDto filter)
        {
            return CurrentDb.Queryable<FlowInstEntity>().Where(f => f.FlowStatus != FlowStatus.Delete)
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(filter.FlowTempId.HasValue, f => f.FlowTempId == filter.FlowTempId)
                .WhereIF(filter.FlowTempTitle.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowTempTitle).Contains(filter.FlowTempTitle.ToLower()))
                .WhereIF(filter.TaskSubject.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.TaskSubject).Contains(filter.TaskSubject.ToLower()))
                .WhereIF(filter.FlowTempName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowTempName).Equals(filter.FlowTempName.ToLower()))
                .WhereIF(filter.FlowNo.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowNo).StartsWith(filter.FlowNo.ToLower()))
                .WhereIF(filter.FlowStepName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowStepName).Equals(filter.FlowStepName.ToLower()))
                .WhereIF(filter.Applicant.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Applicant).Equals(filter.Applicant.ToLower()))
                //.WhereIF(filter.Approver.IsNotNullOrWhiteSpace(), f => f.Approver.Contains(filter.Approver))
                .WhereIF(filter.ApproverName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.ApproverName).Contains(filter.ApproverName.ToLower()))
                .WhereIF(filter.FlowStatus.HasValue, f => f.FlowStatus.Equals(filter.FlowStatus))
                .OrderByDescending(f => f.CreateTime);
        }


        public async Task<FlowInstEntity> GetInfo(Guid flowId)
        {
            var flowInfo = await Get(flowId);
            if (flowInfo == null) throw new AppFriendlyException($"{flowId} process instance does not exist!", "5404");
            var actList = await CurrentDb.Queryable<FlowActInstEntity>().Where(f => f.FlowInstId == flowId).ToListAsync();
            var actRouteList = await CurrentDb.Queryable<FlowInstActRouteEntity>().Where(f => f.FlowInsID == flowId).ToListAsync();
            foreach (var act in actList)
            {
                act.SwitchPath.AddRange(actRouteList.Where(f => f.ActInsID == act.Id));
            }
            flowInfo.FlowActs.AddRange(actList);
            return flowInfo;
        }

        /// <summary>
        /// 流程表单信息，包含表单数据，流程节点，审批记录，附件等
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        /// <exception cref="AppFriendlyException"></exception>
        public async Task<FlowInstInfo> GetFormFlowInfo(Guid flowId)
        {
            var flowInst2 = await Get(flowId);
            var flowInfo = flowInst2.Adapt<FlowInstInfo>();
            if (flowInfo == null) throw new AppFriendlyException($"{flowId}The process instance does not exist！", "5404");

            var actList = await CurrentDb.Queryable<FlowActInstEntity>().OrderBy(f => f.ActStep).Where(f => f.FlowInstId == flowId).ToListAsync();
            flowInfo.FlowActs.AddRange(actList);

            var recordList = await CurrentDb.Queryable<FlowAuditRecordEntity>().Where(f => f.FlowInstId == flowId).OrderBy(f => f.CreateTime).ToListAsync();
            flowInfo.AuditRecords.AddRange(recordList);
            var lastAct = actList.FirstOrDefault(f => f.ActStatus == ActivityStatus.Running);
            if (lastAct != null)
            {
                flowInfo.AuditRecords.AddRange(actList.Where(f => f.ActStep >= lastAct.ActStep).ToList().Adapt<List<FlowAuditRecordEntity>>());
                foreach (var item in flowInfo.AuditRecords.Where(f => f.ActStep == lastAct.ActStep))
                {
                    item.CreateTime = lastAct.UpdateTime.Value;
                }
            }
            else
            {
                var end = actList.FirstOrDefault(f => f.ActName == FlowAction.End.ToString());
                if (end != null)
                {
                    var endNode = end.Adapt<FlowAuditRecordEntity>();
                    endNode.ActOperate = "End";
                    endNode.CreateTime = flowInfo.CompleteTime.HasValue ? flowInfo.CompleteTime.Value : DateTimeOffset.Now;
                    flowInfo.AuditRecords.Add(endNode);
                }
            }

            //foreach (var act in recordList)
            //{
            //    if (act.ActName == "End") continue;
            //    var record = recordList.FirstOrDefault(f => f.ActName == act.ActName);
            //    if (record == null) record = act.Adapt<FlowAuditRecordEntity>();
            //    flowInfo.AuditRecords.Add(record);
            //}
            //flowInfo.AuditRecords.AddRange(recordList);

            var attachmentsList = await CurrentDb.Queryable<FlowAttachmentEntity>().Where(f => f.FlowInstId == flowId).ToListAsync();
            flowInfo.Attacchments.AddRange(attachmentsList.Adapt<List<FlowAttachmentInfo>>());

            return flowInfo;
        }

        /// <summary>
        /// 流程表单信息，包含表单数据，流程节点，审批记录，附件等
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /// <exception cref="AppFriendlyException"></exception>
        public async Task<FlowInstInfo> GetFormFlowInfoByTaskId(Guid taskId)
        {
            var taskInfo = await Get<FlowTodoEntity>(taskId);
            if (taskInfo == null) throw new AppFriendlyException($"{taskId} The task does not exist！", "5404");
            return await GetFormFlowInfo(taskInfo.FlowInstID.Value);
        }

        public async Task<int> Delete(string flowNo)
        {
            flowNo = flowNo?.ToLower();
            using (var uow = CurrentDb.CreateContext())
            {
                var flowInst = await CurrentDb.Queryable<FlowInstEntity>()
                .Where(f => SqlFunc.ToLower(f.FlowNo) == flowNo).FirstAsync();

                await CurrentDb.Updateable<FlowActInstEntity>()
                 .Where(f => f.FlowInstId == flowInst.Id && f.ActStatus == ActivityStatus.Running)
                 .SetColumns(f => new FlowActInstEntity() { ActStatus = ActivityStatus.Stop }).ExecuteCommandAsync();

                await CurrentDb.Updateable<FlowTodoEntity>()
                 .Where(f => f.FlowInstID == flowInst.Id && f.Status == TodoStatus.Todo)
                 .SetColumns(f => new FlowTodoEntity() { Status = TodoStatus.Error, FlowStatus = FlowStatus.Stop }).ExecuteCommandAsync();

                await CurrentDb.Updateable<EmailSendRecordEntity>()
                 .Where(f => f.FlowInstId == flowInst.Id && (f.Status == 0 || f.Status == -1))
                 .SetColumns(f => new EmailSendRecordEntity() { Status = (int)TodoStatus.Error }).ExecuteCommandAsync();


                await CurrentDb.Updateable<FlowInstEntity>()
                 .Where(f => SqlFunc.ToLower(f.FlowNo) == flowNo)
                 .SetColumns(f => new FlowInstEntity() { FlowStatus = FlowStatus.Stop }).ExecuteCommandAsync();

                uow.Commit();
                return 1;
            }
        }

        public override async Task<bool> Delete(Guid id, bool clearCache = true)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                await CurrentDb.Updateable<FlowActInstEntity>()
                 .Where(f => f.FlowInstId == id && f.ActStatus == ActivityStatus.Running)
                 .SetColumns(f => new FlowActInstEntity() { ActStatus = ActivityStatus.Stop }).ExecuteCommandAsync();

                await CurrentDb.Updateable<FlowTodoEntity>()
                 .Where(f => f.FlowInstID == id && f.Status == TodoStatus.Todo)
                 .SetColumns(f => new FlowTodoEntity() { Status = TodoStatus.Error, FlowStatus = FlowStatus.Delete }).ExecuteCommandAsync();

                await CurrentDb.Updateable<EmailSendRecordEntity>()
                 .Where(f => f.FlowInstId == id && (f.Status == 0 || f.Status == -1))
                 .SetColumns(f => new EmailSendRecordEntity() { Status = (int)TodoStatus.Error }).ExecuteCommandAsync();


                await CurrentDb.Updateable<FlowInstEntity>()
                 .Where(f => f.Id == id)
                 .SetColumns(f => new FlowInstEntity() { FlowStatus = FlowStatus.Delete }).ExecuteCommandAsync();

                uow.Commit();
            }
            return true;
        }


        /// <summary>
        /// 申请清单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<PageResult<FlowEntity>> PageMyRequests(FlowInstDto filter)
        {
            filter.Applicant = CurrentUser.Id;
            var query2 = BuildFilterQuery(filter);//.OrderBy(f => f.Id);
            var query = query2.Select<FlowEntity>();
            return new PageResult<FlowEntity>(query.Count(), await Page(query, filter).ToListAsync(), filter.PageNum, filter.PageSize);
        }


        /// <summary>
        /// 申请清单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<PageResult<FlowEntity>> WorkListPageQuery(FlowInstDto filter)
        {
            var query2 = CurrentDb.Queryable<FlowInstEntity>()
                .Where(f => f.FlowStatus != FlowStatus.Delete)
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(filter.FlowTempId.HasValue, f => f.FlowTempId == filter.FlowTempId)
                .WhereIF(filter.FlowTempTitle.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowTempTitle).Contains(filter.FlowTempTitle.ToLower()))
                .WhereIF(filter.TaskSubject.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.TaskSubject).Contains(filter.TaskSubject.ToLower()))
                .WhereIF(filter.FlowTempName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowTempName).Equals(filter.FlowTempName.ToLower()))
                .WhereIF(filter.FlowNo.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowNo).StartsWith(filter.FlowNo.ToLower()))
                .WhereIF(filter.FlowStepName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.FlowStepName).Equals(filter.FlowStepName.ToLower()))
                .WhereIF(filter.Applicant.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Applicant).Equals(filter.Applicant.ToLower()))
                //.WhereIF(filter.Approver.IsNotNullOrWhiteSpace(), f => f.Approver.Contains(filter.Approver))
                .WhereIF(filter.ApproverName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.ApproverName).Contains(filter.ApproverName.ToLower()))
                .WhereIF(filter.FlowStatus.HasValue, f => f.FlowStatus.Equals(filter.FlowStatus));
            //.OrderByDescending(f => f.CreateTime);
            var query = query2.InnerJoin(CurrentDb.Queryable<FlowTodoEntity>(), (f, t) => f.Id == t.FlowInstID)
                .Where((f, t) => SqlFunc.ToLower(f.CreateBy) == CurrentUser.UserId.ToLower() || SqlFunc.ToLower(f.Applicant) == CurrentUser.UserId.ToLower() || SqlFunc.ToLower(t.Approver) == CurrentUser.UserId.ToLower()
                || SqlFunc.ToLower(t.OwnerID) == CurrentUser.UserId.ToLower())
                .OrderByDescending(f => f.CreateTime)
               .Select((f, t) => f)
               .Select<FlowEntity>().Distinct();
            //var query = query2.Select<FlowEntity>();
            return new PageResult<FlowEntity>(query.Count(), await Page(query, filter).ToListAsync(), filter.PageNum, filter.PageSize);
        }


        /// <summary>
        /// 根据流程模板创建流程实例
        /// </summary>
        /// <param name="flowData"></param>
        /// <param name="action">默认启动，传保存草稿是，表示保存</param>
        /// <returns></returns>
        /// <exception cref="AppFriendlyException"></exception>
        [Furion.DatabaseAccessor.UnitOfWork()]
        public async Task<IResult> StartFlowInst(StartFlowDto flowData, FlowAction action = FlowAction.Submit)
        {
            var result = new Result<FlowInstInfo>();
            var flowInst = flowData.Adapt<FlowInstEntity>();
            var flowCacheKey = string.Empty;
            int? flowNo = 0;

            using (var uow = CurrentDb.CreateContext())
            {
                if (flowData.Id != Guid.Empty)
                {
                    var data = await Get(flowData.Id);
                    if (data != null)
                    {
                        flowInst = data;
                        flowInst.FormData = flowData.FormData;
                    }
                }
                FlowTemplateDto flowTempDto = null;
                if (flowData.FlowTempId.IsNotNullOrWhiteSpace() && Guid.TryParse(flowData.FlowTempId, out Guid tid)) flowTempDto = await _flowTemplateService.GetTempInfo(tid);
                else if (flowData.FlowTempName.IsNotNullOrEmpty()) flowTempDto = await _flowTemplateService.GetTempInfo(flowData.FlowTempName);
                if (flowTempDto == null)
                {
                    throw new AppFriendlyException("The process template does not exist.", "5404");
                }

                if (flowInst.FlowNo.IsNullOrWhiteSpace())
                {
                    flowCacheKey = $"ITPortal:Flow:{flowTempDto.FlowName}{DateTimeOffset.Now.ToString("yyyyMMdd")}";
                    flowNo = await _cache.GetIntAsync(flowCacheKey, async () => { return await Task.FromResult(1); });
                    flowData.FormNo = flowInst.FlowNo = $"{flowTempDto.FlowNo.PadRight(8, ' ').Substring(0, 6).Trim()}{DateTimeOffset.Now.ToString("yyyyMMdd")}{(++flowNo):D4}".Trim();// Guid.NewGuid().ToString();
                }
                var flowInstParamDict = FlowExtensions.GetFlowParamDict(flowInst);
                flowInst.FlowStatus = action == FlowAction.Submit ? FlowStatus.Running : FlowStatus.Draft;
                flowInst.FlowTempId = flowTempDto.Id;
                flowInst.FlowTempName = flowTempDto.FlowName;
                flowInst.FlowTempTitle = flowTempDto.FlowTitle;
                flowInst.TaskSubject = ApiEmailHelper.FillTemplate(flowTempDto.TaskTitle, flowData.FormData, flowInstParamDict); //flowTempDto.TaskTitle;//
                flowInst.FormContext = flowTempDto.FormTemplate;
                flowInst.EmailTempID = flowTempDto.EmailTempID;
                flowInst.FormUrl = ApiEmailHelper.FillTemplate(flowTempDto.FormUrl, flowData.FormData, flowInstParamDict);
                flowInst.NoticeType = FlowNoticeType.Email;
                if (flowData.FormData != null) flowInst.FormData = flowData.FormData.ToString();
                else flowInst.FormData = "{}";
                flowInst.CallBackUrl = flowTempDto.CallBackUrl;
                flowInst.Applicant = flowData.Applicant;
                flowInst.ApplicantName = flowData.ApplicantName;
                if (flowData.FormId == Guid.Empty)
                    flowData.FormId = Guid.NewGuid();
                flowInst.Id = flowData.FormId;

                if (action == FlowAction.Submit || action == FlowAction.Start)
                {
                    CopyTempActToActInst(flowInst, flowTempDto.FlowActs, flowData.ActApprovers);

                    //更新开始节点状态为已审批状态
                    var startAct = FlowExtensions.GetStartAct(flowInst.FlowActs);
                    startAct.ActStatus = ActivityStatus.Approval;
                    startAct.Approver = new List<StaffInfo>() { CurrentUser.Adapt<StaffInfo>() };
                    startAct.ApproverName = CurrentUser.Name;

                    //获取下一个审批节点并发送通知
                    var nextAct = flowInst.FlowActs.FirstOrDefault(f => f.ActName == FlowExtensions.GetNextActName(flowTempDto.FlowActs, FlowAction.Start.ToString(), FlowAction.Submit));
                    if (nextAct == null)
                    {
                        throw new AppFriendlyException("The process configuration is abnormal. Please configure the start node name to Start or ActStep to 1", "5002");
                    }
                    nextAct.ActStatus = ActivityStatus.Running;
                    //更新状态
                    flowInst.Approver = nextAct.Approver;
                    flowInst.ApproverName = nextAct.ApproverName = string.Join(";", nextAct.Approver.Select(f => f.Name).Distinct());
                    flowInst.FlowStep = nextAct.ActStep;
                    flowInst.FlowStepName = nextAct.ActName;
                    flowInst.FlowStepTitle = nextAct.ActTitle;
                    await Create(flowInst);

                    //保存附件
                    await AddAttacchments(flowData.Attacchments, flowInst);

                    //添加审批记录
                    var flowAudit = new FlowAuditDto()
                    {
                        Id = Guid.NewGuid(),
                        ActOperate = FlowAction.Submit.ToString(),
                        AuditContent = "Submit",
                    };
                    await AddAuditRecord(flowAudit, startAct);

                    //保存数据
                    foreach (var item in flowInst.FlowActs)
                    {
                        await Create(item);
                        foreach (var routeItem in item.SwitchPath)
                        {
                            routeItem.NextActInsID = flowInst.FlowActs.FirstOrDefault(aitem => aitem.ActName == routeItem.NextActInsName)?.Id;
                            await Create(routeItem);
                        }
                    }

                    //发送通知邮件
                    //创建代办。发送代办邮件通知，ems通知等
                    await SendNotice(flowInst, nextAct, flowData.FormData);

                    var callbackResult = await FlowExtensions.CallBack(_employeeProxyService, flowInst, startAct, flowAudit, action);
                    if (callbackResult.Success)
                    {
                        uow.Commit();
                        if (flowNo > 0)
                            _cache.SetInt(flowCacheKey, flowNo.Value, TimeSpan.FromDays(1));
                    }
                    else
                        result.AddMsg(callbackResult);

                }
                else if (action == FlowAction.Draft)
                {
                    await Create(flowInst);

                    //保存附件
                    await AddAttacchments(flowData.Attacchments, flowInst);

                    uow.Commit();

                    if (flowNo > 0)
                        _cache.SetInt(flowCacheKey, flowNo.Value, TimeSpan.FromDays(1));
                }
            }

            result.Data = flowInst.Adapt<FlowInstInfo>();
            result.Data.Attacchments = flowData.Attacchments;
            return await Task.FromResult(result);
        }


        public async Task AddAttacchments(StartFlowDto flowData, FlowInstEntity flowInst)
        {
            await AddAttacchments(flowData.Attacchments, flowInst);
            //foreach (var item in flowData.Attacchments)
            //{
            //    if (item.IsNeed && item.FileUrl.IsNullOrWhiteSpace())
            //        throw new AppFriendlyException($"{item.FileName} attachment is empty", "5201");
            //    var data = item.Adapt<FlowAttachmentEntity>();
            //    data.FlowInstId = flowInst.Id;
            //    data.FlowTempId = flowInst.FlowTempId.Value;
            //    data.FlowTempName = flowInst.FlowTempName;
            //    if (item.Id == Guid.Empty)
            //        await Create(data);
            //    else
            //        await ModifyHasChange(data);
            //}
        }

        public async Task AddAttacchments(List<FlowAttachmentInfo> attacchments, FlowInstEntity flowInst)
        {
            var attachmentsList = await CurrentDb.Queryable<FlowAttachmentEntity>().Where(f => f.FlowInstId == flowInst.Id).ToListAsync();
            foreach (var item in attacchments)
            {
                if (item.IsNeed && item.FileUrl.IsNullOrWhiteSpace())
                    throw new AppFriendlyException($"{item.FileName} attachment is empty", "5201");
                var data = item.Adapt<FlowAttachmentEntity>();
                data.FlowInstId = flowInst.Id;
                data.FlowTempId = flowInst.FlowTempId.Value;
                data.FlowTempName = flowInst.FlowTempName;
                if (item.Id == Guid.Empty)
                    await Create(data);
                else
                {
                    await ModifyHasChange(data);
                    var file = attachmentsList.FirstOrDefault(f => f.Id == data.Id);
                    if (file != null) attachmentsList.Remove(file);
                }
            }
            if (attachmentsList.Count > 0)
                await CurrentDb.Deleteable<FlowAttachmentEntity>().Where(d => attachmentsList.Select(f => f.Id).Contains(d.Id)).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 发送待办通知
        /// </summary>
        /// <param name="flowInst"></param>
        /// <param name="nextActInst"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public async Task SendNotice(FlowInstEntity flowInst, FlowActInstEntity nextActInst, dynamic formData)//
        {
            formData = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(flowInst.FormData);

            var flowParams = FlowExtensions.GetFlowParamDict(flowInst);
            var todo = await SendTodoNotice(flowInst, nextActInst, formData, flowParams);
            var noticeType = (FlowNoticeType)flowInst.NoticeType;
            if ((noticeType & FlowNoticeType.Email) == FlowNoticeType.Email)
            {
                await SendEmailNotice(flowInst, nextActInst, todo, formData, flowParams);
            }
        }

        /// <summary>
        /// 发送待办通知
        /// </summary>
        /// <param name="flowInst"></param>
        /// <param name="nxtActInst"></param>
        /// <param name="formData"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public async Task<List<FlowTodoEntity>> SendTodoNotice(FlowInstEntity flowInst, FlowActInstEntity nxtActInst, JObject formData, Dictionary<string, object> paramData = null)
        {
            var todoList = new List<FlowTodoEntity>();
            if (nxtActInst == null) return todoList;
            foreach (var item in nxtActInst.Approver)
            {
                var todo = new FlowTodoEntity
                {
                    FlowInstID = flowInst.Id,
                    FlowNo = flowInst.FlowNo,
                    FlowTempID = flowInst.FlowTempId,
                    FlowTempName = flowInst.FlowTempName,
                    OwnerID = item.Ntid,
                    ActID = nxtActInst.Id,
                    ActStep = nxtActInst.ActStep,
                    ActTitle = nxtActInst.ActTitle,
                    ActName = nxtActInst.ActName,
                    Title = ApiEmailHelper.FillTemplate(flowInst.TaskSubject.IsNullOrWhiteSpace() ? flowInst.FlowTempTitle : flowInst.TaskSubject, formData, FlowExtensions.GetFlowParamDict(flowInst, paramData)),
                    Context = flowInst.FormContext,
                    Applicant = flowInst.Applicant,
                    ApplicantName = flowInst.ApplicantName,
                    Approver = item.Ntid,
                    ApproverName = item.Name,
                    FlowStatus = flowInst.FlowStatus.Value,
                    Status = TodoStatus.Todo,
                    NoticeType = flowInst.NoticeType,
                };
                await Create(todo);
                todoList.Add(todo);
            }
            return todoList;
        }

        /// <summary>
        /// 发送待办通知
        /// </summary>
        /// <param name="flowInst"></param>
        /// <param name="nextActInst"></param>
        /// <param name="todo"></param>
        /// <param name="formData"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public async Task SendEmailNotice(FlowInstEntity flowInst, FlowActInstEntity nextActInst, List<FlowTodoEntity> todo, dynamic formData, Dictionary<string, object> paramData = null)
        {
            IResult result = new Result();
            var noticeType = (FlowNoticeType)flowInst.NoticeType;
            if (nextActInst == null) return;
            if ((noticeType & FlowNoticeType.Email) == FlowNoticeType.Email)
            {
                var emailTempId = nextActInst.EmailTempID;
                if (!emailTempId.HasValue || emailTempId.Value == Guid.Empty) emailTempId = flowInst.EmailTempID;
                EmailTemplateEntity emailTemp = await CurrentDb.Queryable<EmailTemplateEntity>().FirstAsync(f => f.Id == emailTempId);

                if (nextActInst.ActName == FlowAction.End.ToString())
                    nextActInst.Approver.Add(new StaffInfo() { Ntid = flowInst.Applicant, Name = flowInst.ApplicantName });

                if (nextActInst.Approver.Count == 0)
                {
                    return;
                }
                if (emailTemp == null)
                {
                    return;
                }

                var emailData = emailTemp.Adapt<EmailSendRecordEntity>();
                emailData.Id = Guid.NewGuid();
                emailData.FlowInstId = flowInst.Id;
                emailData.ActionId = nextActInst.Id;
                emailData.Status = 0;
                var emailToList = new List<string>();
                foreach (var item in nextActInst.Approver)
                {
                    var email = item.Email;
                    if (item.Email.IsNullOrWhiteSpace())
                    {
                        email = await GetStafEmail(item, nextActInst);
                    }
                    emailToList.Add(email);
                }
                if (emailData.EmailTo.IsNotNullOrWhiteSpace()) emailToList.Add(emailData.EmailTo.Trim(';').Trim());
                emailData.EmailTo = string.Join(";", emailToList.Distinct().ToArray());
                emailData = ApiEmailHelper.FillEmailTemplateByList(emailData, formData, paramData);

                await Create(emailData);
            }
        }



        private async Task<string> GetStafEmail(StaffInfo item, FlowActInstEntity nextActInst)
        {
            bool isGetUserInfoError = false;
            var email = string.Empty;
            try
            {
                var userInfo = await _employeeProxyService.GetEmployeeAsync(item.Ntid);
                if (userInfo.WorkEmail.IsNotNullOrWhiteSpace())
                {
                    email = userInfo.WorkEmail;
                }
                else
                {
                    isGetUserInfoError = true;
                }
            }
            catch (Exception ex)
            {
                isGetUserInfoError = true;
                _logger.LogError(ex, ex.Message);
            }
            if (email.IsNullOrWhiteSpace())
            {
                throw new AppFriendlyException($"The next approval node [{nextActInst.ActName}] approver [{item.Name}] lacks email configuration! Please contact the system administrator to maintain the personnel information {(isGetUserInfoError ? ", get the current approver employee information exception!" : "")}", "5104");
            }
            return email;
        }


        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="info"></param>
        public async Task<IResult> Approval(FlowAuditDto info)
        {
            return await Submit(info, FlowAction.Approval);
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="info"></param>
        public async Task<IResult> Reject(FlowAuditDto info)
        {
            return await Submit(info, FlowAction.Reject);
        }

        /// <summary>
        /// 驳回到开始节点
        /// </summary>
        /// <param name="info"></param>
        public async Task<IResult> RejectStart(FlowAuditDto info)
        {
            return await Submit(info, FlowAction.RejectStart);
        }


        /// <summary>
        /// 流程拒绝，审批结束
        /// </summary>
        /// <param name="info"></param>
        public async Task<IResult> RejectEnd(FlowAuditDto info)
        {
            return await Submit(info, FlowAction.RejectEnd);
        }

        /// <summary>
        /// 转办
        /// </summary>
        /// <param name="info"></param>
        public async Task<IResult> Transfer(FlowTransferAuditDto info)
        {
            return await TransferSubmit(info, FlowAction.Transfer);
        }

        /// <summary>
        /// 跳转到指定节点
        /// </summary>
        /// <param name="info"></param>
        public async Task<IResult> GotToAct(FlowGotToActAuditDto info)
        {
            return await GotToActName(info);
        }

        /// <summary>
        /// 转办，
        /// </summary>
        /// <param name="info"></param>
        /// <param name="action"></param>
        public async Task<IResult> TransferSubmit(FlowTransferAuditDto info, FlowAction action)
        {
            var result = new Result<string>();
            using (var uow = CurrentDb.CreateContext())
            {
                FlowTodoEntity curTodo = null;
                List<FlowTodoEntity> todoList = new List<FlowTodoEntity>();
                if (info.Id.HasValue && info.Id.Value != Guid.Empty)
                    curTodo = await CurrentDb.Queryable<FlowTodoEntity>().FirstAsync(f => f.Id == info.Id);
                else if (info.FlowInstId.HasValue && info.FlowInstId != Guid.Empty)
                {
                    todoList = await CurrentDb.Queryable<FlowTodoEntity>().OrderByDescending(f => f.CreateTime).Where(f => f.FlowInstID == info.FlowInstId && f.Status == TodoStatus.Todo).ToListAsync();
                    if (todoList.Count > 0)
                    {
                        curTodo = todoList.FirstOrDefault(f => f.OwnerID.Equals(this.CurrentUser.Id, StringComparison.CurrentCultureIgnoreCase));
                    }
                }
                // 01 close todo 
                if (curTodo == null)
                {
                    result.SetError("To-do not exist！");
                    return result;
                }
                else if (curTodo.Status != TodoStatus.Todo)
                {
                    result.SetError("To-do items have been processed or cancelled！");
                    return result;
                }
                else if ((!curTodo.Approver.Equals(CurrentUser.NtId, StringComparison.CurrentCultureIgnoreCase) && CurrentUser.NtId != "AutoApproveUser") && action != FlowAction.Cancel)
                {
                    result.SetError("No permission to operate！");
                    return result;
                }
                curTodo.Status = (TodoStatus)action;
                curTodo.CompleteTime = DateTimeOffset.Now;

                await Modify(curTodo);

                //01 get flow inst
                var flowInst = await GetInfo(curTodo.FlowInstID.Value);
                if (flowInst.Applicant != CurrentUser.NtId && action == FlowAction.Cancel)
                {
                    result.SetError("No permission to operate！");
                    return result;
                }

                //02 get current action
                var currentAct = flowInst.FlowActs.FirstOrDefault(f => f.Id == curTodo.ActID);
                if (currentAct == null)
                {
                    result.SetError("The process node does not exist! Data abnormality");
                    return result;
                }
                if (currentAct.ActStatus != ActivityStatus.Running)
                {
                    result.SetError($"Process node status is abnormal:{currentAct.ActStatus}！Data abnormality");
                    return result;
                }


                //currentAct.ActStatus = (ActivityStatus)action;
                //currentAct.CompleteTime = DateTimeOffset.Now;

                var empUser = await _employeeProxyService.GetEmployeeAsync(info.Transferor);
                if (empUser == null)
                {
                    result.SetError($"转办人不存在:{info.TransferorName}({info.Transferor})！");
                    return result;
                }
                //移除当前处理人
                currentAct.Approver.Remove(currentAct.Approver.First(f => f.Ntid.Equals(curTodo.Approver, StringComparison.CurrentCultureIgnoreCase)));

                //转办人已经存在，直接移除，不添加
                if (todoList != null && todoList.Count > 1 && todoList.Any(f => f.Approver.Equals(empUser.WorkNTID, StringComparison.CurrentCultureIgnoreCase)))
                {
                    flowInst.Approver = currentAct.Approver;
                    flowInst.ApproverName = currentAct.ApproverName = string.Join(";", currentAct.Approver.Select(f => f.Name));

                    await Modify(flowInst);

                    // 06 add audit record
                    info.ActOperate = action.ToString();
                    await AddAuditRecord(info, currentAct);

                    uow.Commit();
                    return result;
                }
                //添加转办人
                currentAct.Approver.Add(empUser.Adapt<StaffInfo>());
                flowInst.ApproverName = currentAct.ApproverName = string.Join(";", currentAct.Approver.Select(f => f.Name));
                flowInst.Approver = currentAct.Approver;

                //03 get next action
                await Modify(currentAct);
                await Modify(flowInst);

                //04 add todo list 
                var nextAct = currentAct.Adapt<FlowActInstEntity>();
                nextAct.Approver = new List<StaffInfo>() { empUser.Adapt<StaffInfo>() };
                nextAct.ApproverName = empUser.Name;
                await SendNotice(flowInst, nextAct, flowInst.FormData);

                // 06 add audit record
                info.ActOperate = action.ToString();
                await AddAuditRecord(info, currentAct);

                uow.Commit();
            }

            return result;
        }


        /// <summary>
        /// 审批
        /// update by 25/07/17 扩展支持多人审批
        /// </summary>
        /// <param name="info"></param>
        /// <param name="action"></param>
        public async Task<IResult> Submit(FlowAuditDto info, FlowAction action)
        {
            var result = new Result<string>();
            using (var uow = CurrentDb.CreateContext())
            {
                FlowInstEntity flowInst = null;
                FlowTodoEntity curTodo = null;
                List<FlowTodoEntity> todoList = new List<FlowTodoEntity>();
                int totalTodo = 0;
                var flwoInstId = info.FlowInstId;
                if (info.Id.HasValue && info.Id.Value != Guid.Empty)
                {
                    curTodo = await CurrentDb.Queryable<FlowTodoEntity>().FirstAsync(f => f.Id == info.Id);
                    if (curTodo != null) flwoInstId = curTodo.FlowInstID;
                }
                if (info.FlowInstId.HasValue && info.FlowInstId != Guid.Empty)
                {
                    flowInst = await GetInfo(info.FlowInstId.Value);
                    if (flowInst == null) throw new AppFriendlyException("flow Inst not exist!", 5503);
                    todoList = await CurrentDb.Queryable<FlowTodoEntity>().OrderByDescending(f => f.CreateTime)
                        .Where(f => f.FlowInstID == info.FlowInstId && flowInst.FlowStepName == f.ActName).ToListAsync();
                    totalTodo = todoList.Count();
                    todoList = todoList.Where(f => f.Status == TodoStatus.Todo).ToList();
                    if (todoList.Count > 0)
                    {
                        curTodo = todoList.FirstOrDefault(f => f.OwnerID.Equals(this.CurrentUser.Id, StringComparison.CurrentCultureIgnoreCase));
                        if (curTodo == null) curTodo = todoList.FirstOrDefault();
                    }
                }
                // 01 close todo 
                if (curTodo == null)
                {
                    result.SetError("To-do not exist!");
                    return result;
                }
                else if (curTodo.Status != TodoStatus.Todo)
                {
                    result.SetError("To-do items have been processed or cancelled!");
                    return result;
                }
                else if (!curTodo.Approver.Equals(CurrentUser.NtId, StringComparison.CurrentCultureIgnoreCase) && CurrentUser.NtId != "AutoApproveUser" && action != FlowAction.Cancel)
                {
                    result.SetError("No permission to operate!");
                    return result;
                }
                curTodo.Status = (TodoStatus)action;
                curTodo.CompleteTime = DateTimeOffset.Now;


                //01 get flow inst
                if (flowInst == null) flowInst = await GetInfo(curTodo.FlowInstID.Value);
                if (flowInst.Applicant != CurrentUser.NtId && action == FlowAction.Cancel)
                {
                    result.SetError("No permission to operate!");
                    return result;
                }

                //02 get current action
                var currentAct = flowInst.FlowActs.FirstOrDefault(f => f.Id == curTodo.ActID);
                if (currentAct == null)
                {
                    result.SetError("The process node does not exist! Data abnormality");
                    return result;
                }
                if (currentAct.ActStatus != ActivityStatus.Running)
                {
                    result.SetError($"Process node status is abnormal:{currentAct.ActStatus}！Data abnormality");
                    return result;
                }
                //多人审批时
                bool GoToNext = false;
                if (CurrentUser.NtId != "AutoApproveUser" && currentAct.HitTimes > 1 && totalTodo > 1 && (ActivityStatus)action == ActivityStatus.Approval)
                {
                    currentAct.ApprovedTimes++;
                    if (CheckHit(currentAct, totalTodo))
                    {
                        currentAct.ActStatus = (ActivityStatus)action;
                        currentAct.CompleteTime = DateTimeOffset.Now;
                        GoToNext = true;
                    }
                }
                else
                {
                    currentAct.ActStatus = (ActivityStatus)action;
                    currentAct.CompleteTime = DateTimeOffset.Now;
                    if ((ActivityStatus)action != ActivityStatus.Transfer)
                        GoToNext = true;
                }

                if (GoToNext)
                {
                    //所有待办取消
                    foreach (var item in todoList)
                    {
                        if (curTodo.Id == item.Id) continue;
                        item.Status = TodoStatus.Cancel;
                        item.CompleteTime = DateTimeOffset.Now;
                        await Modify(item);
                    }
                }
                else if (curTodo.Status == TodoStatus.Cancel)
                {
                    //所有待办撤销
                    foreach (var item in todoList)
                    {
                        item.Status = (TodoStatus)action;
                        item.CompleteTime = DateTimeOffset.Now;
                        await Modify(item);
                    }
                }

                await Modify(curTodo);
                await Modify(currentAct);

                //03 get next action
                if (GoToNext)
                {
                    FlowActInstEntity nextAct = null;
                    if (action == FlowAction.RejectStart) nextAct = FlowExtensions.GetStartAct(flowInst.FlowActs);
                    else if (action == FlowAction.RejectEnd) nextAct = FlowExtensions.GetEndAct(flowInst.FlowActs);
                    else if (action == FlowAction.Cancel)
                    {
                        nextAct = FlowExtensions.GetEndAct(flowInst.FlowActs);
                    }
                    else
                    {
                        if (currentAct.ActName == ActType.Start.ToString()) action = FlowAction.Submit;
                        var nextActName = FlowExtensions.SwitchNextActName(currentAct, action);
                        nextAct = flowInst.FlowActs.FirstOrDefault(f => f.ActName == nextActName);
                    }

                    if (nextAct != null)
                    {
                        if (nextAct.ActName == FlowAction.End.ToString())
                        {
                            nextAct.ActStatus = ActivityStatus.Approval;
                            flowInst.FlowStatus = FlowStatus.Completed;
                            flowInst.CompleteTime = DateTimeOffset.Now;
                        }
                        else
                        {
                            nextAct.ActStatus = ActivityStatus.Running;
                            nextAct.ApprovedTimes = 0;
                        }
                        flowInst.FlowStep = nextAct.ActStep;
                        flowInst.FlowStepName = nextAct.ActName;
                        flowInst.FlowStepTitle = nextAct.ActTitle;
                        if (nextAct.ActName == FlowAction.Start.ToString())
                        {
                            var empUser = await _employeeProxyService.GetEmployeeAsync(flowInst.Applicant);
                            flowInst.Approver = nextAct.Approver = new List<StaffInfo>() { empUser.Adapt<StaffInfo>() };
                            flowInst.ApproverName = nextAct.ApproverName = empUser.Name;
                        }
                        else
                        {
                            if (info.FlowActs?.Any(f => f.ActName == nextAct.ActName && f.ActorParms?.Count > 0) == true)
                            {
                                var tmpActApprover = info.FlowActs.First(f => f.ActName == nextAct.ActName);
                                nextAct.Approver = tmpActApprover.ActorParms;
                                nextAct.ApproverName = tmpActApprover.ActorParmsName;
                            }

                            flowInst.Approver = nextAct.Approver;
                            flowInst.ApproverName = nextAct.ApproverName;
                        }

                        await Modify(nextAct);
                        //如果存其他节点审批设置，修改节点审批人
                        await UpdateActor(nextAct, info, flowInst);
                    }
                    else if (action == FlowAction.Cancel)
                    {
                        flowInst.FlowStatus = FlowStatus.Cancel;
                        flowInst.CompleteTime = DateTimeOffset.Now;
                        flowInst.FlowStep = 99;
                        flowInst.FlowStepName = FlowAction.Cancel.ToString();
                        flowInst.Approver = null;
                        flowInst.ApproverName = "";
                    }
                    else
                    {
                        if (action == FlowAction.Reject || action == FlowAction.RejectEnd)
                        {
                            flowInst.FlowStatus = FlowStatus.Reject;
                        }
                        else
                        {
                            flowInst.FlowStatus = currentAct.ActName == FlowAction.End.ToString() ? FlowStatus.Completed : FlowStatus.Stop;
                        }

                        flowInst.CompleteTime = DateTimeOffset.Now;
                        flowInst.FlowStep = 99;
                        flowInst.FlowStepName = FlowAction.End.ToString();
                        flowInst.Approver = null;
                        flowInst.ApproverName = "";
                    }
                    await Modify(flowInst);


                    //04 add todo list 
                    await SendNotice(flowInst, nextAct, flowInst.FormData);
                }
                else
                {
                    //update approver
                    flowInst.Approver = flowInst.Approver.Where(f => todoList.Where(d => d.Status == 0).Select(d => d.Approver).Contains(f.Ntid)).ToList();
                    flowInst.ApproverName = string.Join(';', flowInst.Approver.Select(f => f.Name).Distinct());
                    await Modify(flowInst);
                }


                //保存附件
                await AddAttacchments(info.Attacchments, flowInst);

                // 06 add audit record
                info.ActOperate = action.ToString();
                await AddAuditRecord(info, currentAct);

                if (GoToNext)
                {
                    var callbackResult = await FlowExtensions.CallBack(_employeeProxyService, flowInst, currentAct, info, action);
                    if (callbackResult.Success)
                        uow.Commit();
                    else
                        result.AddMsg(callbackResult);
                }
                else
                {
                    uow.Commit();
                }
            }

            return result;
        }


        ///// <summary>
        ///// 审批操作
        ///// update by 25/07/17 扩展支持多人审批
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="action"></param>
        //public async Task<IResult> Approval(FlowAuditDto info, FlowAction action)
        //{
        //    var result = new Result<string>();
        //    using (var uow = CurrentDb.CreateContext())
        //    {
        //        var todoInfo = await GetTaskList(info, action);
        //        var curTask = todoInfo.curTask;
        //        curTask.Status = (TodoStatus)action;
        //        curTask.CompleteTime = DateTimeOffset.Now;

        //        //01 get flow inst
        //        var flowInst = await GetInfo(curTask.FlowInstID.Value);
        //        if (flowInst.Applicant != CurrentUser.NtId && action == FlowAction.Cancel)
        //            throw new AppFriendlyException("No permission to operate!", 5401);

        //        //02 get current action
        //        var currentAct = flowInst.FlowActs.FirstOrDefault(f => f.Id == curTask.ActID);
        //        if (currentAct == null)
        //            throw new AppFriendlyException("The process node does not exist! Data abnormality", 5404);

        //        if (currentAct.ActStatus != ActivityStatus.Running)
        //            throw new AppFriendlyException($"Process node status is abnormal:{currentAct.ActStatus}！Data abnormality", 5302);

        //        //检查审批是否通过
        //        bool isPass = CheckApprovalPass(currentAct, todoInfo.TotalTodo, action);
        //        await DealTask(todoInfo.taskList, isPass, action);

        //        await Modify(curTask);
        //        await Modify(currentAct);

        //        //03 get next action
        //        if (isPass)
        //        {
        //            FlowActInstEntity nextAct = null;
        //            if (action == FlowAction.RejectStart) nextAct = FlowExtensions.GetStartAct(flowInst.FlowActs);
        //            else if (action == FlowAction.RejectEnd) nextAct = FlowExtensions.GetEndAct(flowInst.FlowActs);
        //            else if (action == FlowAction.Cancel)
        //            {
        //                nextAct = FlowExtensions.GetEndAct(flowInst.FlowActs);
        //            }
        //            else
        //            {
        //                if (currentAct.ActName == ActType.Start.ToString()) action = FlowAction.Submit;
        //                var nextActName = FlowExtensions.SwitchNextActName(currentAct, action);
        //                nextAct = flowInst.FlowActs.FirstOrDefault(f => f.ActName == nextActName);
        //            }

        //            if (nextAct != null)
        //            {
        //                if (nextAct.ActName == FlowAction.End.ToString())
        //                {
        //                    nextAct.ActStatus = ActivityStatus.Approval;
        //                    flowInst.FlowStatus = FlowStatus.Completed;
        //                    flowInst.CompleteTime = DateTimeOffset.Now;
        //                }
        //                else
        //                {
        //                    nextAct.ActStatus = ActivityStatus.Running;
        //                    nextAct.ApprovedTimes = 0;
        //                }
        //                flowInst.FlowStep = nextAct.ActStep;
        //                flowInst.FlowStepName = nextAct.ActName;
        //                flowInst.FlowStepTitle = nextAct.ActTitle;
        //                if (nextAct.ActName == FlowAction.Start.ToString())
        //                {
        //                    var empUser = await _employeeProxyService.GetEmployeeAsync(flowInst.Applicant);
        //                    flowInst.Approver = nextAct.Approver = new List<StaffInfo>() { empUser.Adapt<StaffInfo>() };
        //                    flowInst.ApproverName = nextAct.ApproverName = empUser.Name;
        //                }
        //                else
        //                {
        //                    flowInst.Approver = nextAct.Approver;
        //                    flowInst.ApproverName = nextAct.ApproverName;
        //                }

        //                await Modify(nextAct);
        //            }
        //            else if (action == FlowAction.Cancel)
        //            {
        //                flowInst.FlowStatus = FlowStatus.Cancel;
        //                flowInst.CompleteTime = DateTimeOffset.Now;
        //                flowInst.FlowStep = 99;
        //                flowInst.FlowStepName = FlowAction.Cancel.ToString();
        //                flowInst.Approver = null;
        //                flowInst.ApproverName = "";
        //            }
        //            else
        //            {
        //                if (action == FlowAction.Reject || action == FlowAction.RejectEnd)
        //                {
        //                    flowInst.FlowStatus = FlowStatus.Reject;
        //                }
        //                else
        //                {
        //                    flowInst.FlowStatus = currentAct.ActName == FlowAction.End.ToString() ? FlowStatus.Completed : FlowStatus.Stop;
        //                }

        //                flowInst.CompleteTime = DateTimeOffset.Now;
        //                flowInst.FlowStep = 99;
        //                flowInst.FlowStepName = FlowAction.End.ToString();
        //                flowInst.Approver = null;
        //                flowInst.ApproverName = "";
        //            }
        //            await Modify(flowInst);


        //            //04 add todo list 
        //            await SendNotice(flowInst, nextAct, flowInst.FormData);
        //        }
        //        else
        //        {
        //            //update approver
        //            flowInst.Approver = flowInst.Approver.Where(f => todoInfo.taskList.Where(d => d.Status == 0).Select(d => d.Approver).Contains(f.Ntid)).ToList();
        //            flowInst.ApproverName = string.Join(';', flowInst.Approver.Select(f => f.Name).Distinct());
        //            await Modify(flowInst);
        //        }


        //        //保存附件
        //        await AddAttacchments(info.Attacchments, flowInst);

        //        // 06 add audit record
        //        info.ActOperate = action.ToString();
        //        await AddAuditRecord(info, currentAct);

        //        if (isPass)
        //        {
        //            var callbackResult = await FlowExtensions.CallBack(_employeeProxyService, flowInst, currentAct, info, action);
        //            if (callbackResult.Success)
        //                uow.Commit();
        //            else
        //                result.AddMsg(callbackResult);
        //        }
        //        else
        //        {
        //            uow.Commit();
        //        }
        //    }

        //    return result;
        //}

        private void CheckTodo(FlowTodoEntity curTodo, FlowAction action)
        {
            // 01 close todo 
            if (curTodo == null)
            {
                throw new AppFriendlyException("To-do not exist!", 5002);
            }
            else if (curTodo.Status != TodoStatus.Todo)
            {
                throw new AppFriendlyException("To-do items have been processed or cancelled!", 5002);
            }
            else if (!curTodo.Approver.Equals(CurrentUser.NtId, StringComparison.CurrentCultureIgnoreCase) && CurrentUser.NtId != "AutoApproveUser" && action != FlowAction.Cancel)
            {
                throw new AppFriendlyException("No permission to operate!", 5401);
            }
        }

        ///// <summary>
        ///// 检查审批是否通过
        ///// </summary>
        ///// <returns></returns>
        //private bool CheckApprovalPass(FlowActInstEntity currentAct,int totalTodo, FlowAction action)
        //{
        //    bool isPass = false;
        //    if (CurrentUser.NtId != "AutoApproveUser" && currentAct.HitTimes > 1 && totalTodo > 1 && (ActivityStatus)action == ActivityStatus.Approval)
        //    {
        //        currentAct.ApprovedTimes++;
        //        if (CheckHit(currentAct, totalTodo))
        //        {
        //            currentAct.ActStatus = (ActivityStatus)action;
        //            currentAct.CompleteTime = DateTimeOffset.Now;
        //            isPass = true;
        //        }
        //    }
        //    else
        //    {
        //        currentAct.ActStatus = (ActivityStatus)action;
        //        currentAct.CompleteTime = DateTimeOffset.Now;
        //        if ((ActivityStatus)action != ActivityStatus.Transfer)
        //            isPass = true;
        //    }
        //    return isPass;
        //}

        ///// <summary>
        ///// 检查审批是否通过
        ///// </summary>
        ///// <returns></returns>
        //private async Task DealTask(List<FlowTodoEntity> taskList, bool isPass, FlowAction action)
        //{
        //    if (isPass)
        //    {
        //        //所有待办取消
        //        foreach (var item in taskList)
        //        {
        //            if (item.Status != TodoStatus.Todo) continue;
        //            item.Status = TodoStatus.Cancel;
        //            item.CompleteTime = DateTimeOffset.Now;
        //            await Modify(item);
        //        }
        //    }
        //    else if ((TodoStatus)action == TodoStatus.Cancel)
        //    {
        //        //所有待办撤销
        //        foreach (var item in taskList)
        //        {
        //            item.Status = (TodoStatus)action;
        //            item.CompleteTime = DateTimeOffset.Now;
        //            await Modify(item);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取审批待办信息
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //private async Task<(List<FlowTodoEntity> taskList, FlowTodoEntity curTask, int TotalTodo)> GetTaskList(FlowAuditDto info, FlowAction action)
        //{
        //    FlowTodoEntity curTodo = null;
        //    List<FlowTodoEntity> todoList = new List<FlowTodoEntity>();
        //    int totalTodo = 0;
        //    var flwoInstId = info.FlowInstId;
        //    if (info.Id.HasValue && info.Id.Value != Guid.Empty)
        //    {
        //        curTodo = await CurrentDb.Queryable<FlowTodoEntity>().FirstAsync(f => f.Id == info.Id);
        //        if (curTodo != null) flwoInstId = curTodo.FlowInstID;
        //    }
        //    if (info.FlowInstId.HasValue && info.FlowInstId != Guid.Empty)
        //    {
        //        todoList = await CurrentDb.Queryable<FlowTodoEntity>().OrderByDescending(f => f.CreateTime).Where(f => f.FlowInstID == info.FlowInstId).ToListAsync();
        //        totalTodo = todoList.Count();
        //        todoList = todoList.Where(f => f.Status == TodoStatus.Todo).ToList();
        //        if (curTodo != null && todoList.Count > 0)
        //        {
        //            curTodo = todoList.FirstOrDefault(f => f.OwnerID.Equals(this.CurrentUser.Id, StringComparison.CurrentCultureIgnoreCase));
        //            if (curTodo == null) curTodo = todoList.FirstOrDefault();
        //        }
        //    }
        //    CheckTodo(curTodo, action);
        //    return (todoList, curTodo, totalTodo);
        //}


        ///// <summary>
        ///// 获取下一个审批节点
        ///// </summary>
        ///// <returns></returns>
        //private async Task<List<FlowActInstEntity>> GetToNextAct(FlowInstEntity flowInst,FlowActInstEntity currentAct, FlowAction action)
        //{
        //    List<FlowActInstEntity> nextActList = new List<FlowActInstEntity>();
        //    if (action == FlowAction.RejectStart) nextActList.Add(FlowExtensions.GetStartAct(flowInst.FlowActs));
        //    else if (action == FlowAction.RejectEnd) nextActList.Add( FlowExtensions.GetEndAct(flowInst.FlowActs));
        //    else if (action == FlowAction.Cancel)
        //    {
        //        nextAct = FlowExtensions.GetEndAct(flowInst.FlowActs);
        //    }
        //    else
        //    {
        //        if (currentAct.ActName == ActType.Start.ToString()) action = FlowAction.Submit;
        //        var nextActName = FlowExtensions.SwitchNextActName(currentAct, action);
        //        nextAct = flowInst.FlowActs.FirstOrDefault(f => f.ActName == nextActName);
        //    }
        //}

        /// <summary>
        /// 审批
        /// update by 25/07/17 扩展支持多人审批
        /// </summary>
        /// <param name="info"></param>
        public async Task<IResult> GotToActName(FlowGotToActAuditDto info)
        {
            var result = new Result<string>();
            using (var uow = CurrentDb.CreateContext())
            {
                Guid flowInstId = Guid.Empty;
                FlowTodoEntity curTodo = null;
                List<FlowTodoEntity> todoList = new List<FlowTodoEntity>();
                if (info.Id.HasValue && info.Id.Value != Guid.Empty)
                {
                    curTodo = await CurrentDb.Queryable<FlowTodoEntity>().FirstAsync(f => f.Id == info.Id);
                    if (curTodo != null)
                    {
                        flowInstId = curTodo.FlowInstID.Value;
                        todoList = await CurrentDb.Queryable<FlowTodoEntity>().OrderByDescending(f => f.CreateTime).Where(f => f.FlowInstID == flowInstId && f.Status == TodoStatus.Todo).ToListAsync();
                    }
                }
                else if (info.FlowInstId.HasValue && info.FlowInstId != Guid.Empty)
                {
                    flowInstId = info.FlowInstId.Value;
                    todoList = await CurrentDb.Queryable<FlowTodoEntity>().OrderByDescending(f => f.CreateTime).Where(f => f.FlowInstID == flowInstId && f.Status == TodoStatus.Todo).ToListAsync();
                }
                if (flowInstId == Guid.Empty)
                {
                    result.SetError("流程不存在！");
                    return result;
                }

                //所有待办撤销
                foreach (var item in todoList)
                {
                    item.Status = TodoStatus.Cancel;
                    item.CompleteTime = DateTimeOffset.Now;
                    await Modify(item);
                }

                //01 get flow inst
                var flowInst = await GetInfo(flowInstId);

                //03 get next action
                var nextAct = flowInst.FlowActs.FirstOrDefault(f => f.ActName == info.ActName);
                if (nextAct == null)
                {
                    result.SetError("The process node does not exist! Data abnormality");
                    return result;
                }
                if (nextAct.ActName == FlowAction.End.ToString())
                {
                    flowInst.FlowStatus = FlowStatus.Completed;
                    flowInst.CompleteTime = DateTimeOffset.Now;
                }
                else
                {
                    nextAct.ActStatus = ActivityStatus.Running;
                    flowInst.FlowStatus = FlowStatus.Running;
                }
                flowInst.FlowStep = nextAct.ActStep;
                flowInst.FlowStepName = nextAct.ActName;
                flowInst.FlowStepTitle = nextAct.ActTitle;
                if (nextAct.ActName == FlowAction.Start.ToString())
                {
                    var empUser = await _employeeProxyService.GetEmployeeAsync(flowInst.Applicant);
                    flowInst.Approver = nextAct.Approver = new List<StaffInfo>() { empUser.Adapt<StaffInfo>() };
                    flowInst.ApproverName = nextAct.ApproverName = empUser.Name;
                }
                else
                {
                    flowInst.Approver = nextAct.Approver;
                    flowInst.ApproverName = nextAct.ApproverName;
                }

                await Modify(nextAct);
                await Modify(flowInst);

                //04 add todo list 
                await SendNotice(flowInst, nextAct, flowInst.FormData);

                uow.Commit();
            }

            return result;
        }


        public async Task AddAuditRecord(FlowAuditDto record, FlowActInstEntity currentActInst)
        {
            var data = record.Adapt<FlowAuditRecordEntity>();
            record.Id = data.Id = Guid.NewGuid();
            data.FlowInstId = currentActInst.FlowInstId;
            data.ActId = currentActInst.Id;
            data.ActStep = currentActInst.ActStep;
            data.ActTitle = currentActInst.ActTitle;
            data.ActName = currentActInst.ActName;

            data.Approver = CurrentUser.UserId;
            data.ApproverName = CurrentUser.Name;

            await Create(data);
        }



        private FlowInstEntity CopyTempActToActInst(FlowInstEntity prosInst, List<FlowTempActDto> tmpActDtos, List<FlowActApprover> actApprovers)
        {
            foreach (var item in tmpActDtos)
            {
                var actInst = CopyTempActToActInst(item, prosInst, actApprovers.FirstOrDefault(f => f.ActName == item.ActName));
                prosInst.FlowActs.Add(actInst);
            }
            return prosInst;
        }

        private FlowActInstEntity CopyTempActToActInst(FlowTempActDto tmpActDto, FlowInstEntity flowInst, FlowActApprover actApprover)
        {
            if ((actApprover == null || actApprover.ActorParms == null || actApprover.ActorParms.Count == 0) && (tmpActDto.ActorParms == null || tmpActDto.ActorParms.Count == 0) && tmpActDto.ActName != FlowAction.Start.ToString() && tmpActDto.ActName != FlowAction.End.ToString())
            {
                throw new AppFriendlyException($"Please configure the [{tmpActDto.ActTitle}] node approver!", "5001");
            }
            var actInst = tmpActDto.Adapt<FlowActInstEntity>();
            actInst.Id = Guid.NewGuid();
            if (tmpActDto.IsLockApprover == true && tmpActDto.ActorParms?.Count > 0)
            {
                actInst.Approver = tmpActDto.ActorParms;
                actInst.ApproverName = tmpActDto.ActorParmsName;
            }
            else
            {
                actInst.Approver = actApprover?.ActorParms.Count() > 0 ? actApprover.ActorParms : tmpActDto.ActorParms;
                actInst.ApproverName = actApprover?.ActorParms.Count() > 0 ? actApprover.ActorParmsName : tmpActDto.ActorParmsName;
            }
            actInst.ActStatus = (int)ActivityStatus.None;
            //if (actInst.Approver != null && actInst.Approver.Count() > 0)
            //    actInst.HitTimes = actInst.Approver.Count();
            actInst.FlowInstId = flowInst.Id;
            //flowInst.FlowActs.Add(actInst);
            actInst.SwitchPath.ForEach(item =>
            {
                item.Id = Guid.NewGuid();
                item.FlowInsID = flowInst.Id;
                item.ActInsID = actInst.Id;
            });

            return actInst;
        }

        //private List<FlowInstActRouteEntity> CopyTempActRouteToActInst( FlowTempActDto tmpActDto,FlowActInstEntity actInst, FlowInstEntity flowInst)
        //{
        //    var list = new List<FlowInstActRouteEntity>();
        //    foreach (var routeItem in tmpActDto.SwitchPath)
        //    {
        //        var routeInst = routeItem.Adapt<FlowInstActRouteEntity>();
        //        routeInst.Id = Guid.NewGuid();
        //        routeInst.FlowInsID = flowInst.Id;
        //        routeInst.ActInsID = actInst.Id;
        //        list.Add(routeInst);
        //    }
        //    return list;
        //}

        /// <summary>
        /// 会签检查审批次数:
        /// </summary>
        /// <param name="currentAct"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private bool CheckHit(FlowActInstEntity currentAct, int total)
        {
            if (currentAct.ApprovedTimes >= total) return true;
            if (currentAct.HitTimes >= 1 && currentAct.ApprovedTimes >= currentAct.HitTimes) return true;//实际多少人通过则通过
            if (currentAct.HitTimes == 20 && currentAct.ApprovedTimes >= 2) return true;//少数人审批通过则通过
            if (currentAct.HitTimes == 50 && currentAct.ApprovedTimes / (float)total >= 0.5) return true;//多数人审批通过则通过
            if (currentAct.HitTimes == 80 && currentAct.ApprovedTimes / (float)total >= 0.66) return true;//大多数人审批通过则通过
            return false;
        }


        private async Task UpdateActor(FlowActInstEntity currentAct, FlowAuditDto auditInfo, FlowInstEntity flowInst)
        {
            if (auditInfo.FlowActs == null || auditInfo.FlowActs.Count == 0) return;
            var actList = new List<FlowActInstEntity>();
            foreach (var item in auditInfo.FlowActs)
            {
                if (item.ActorParms == null || item.ActorParms.Count <= 0 || item.ActStep <= currentAct.ActStep) continue;
                var act = flowInst.FlowActs.FirstOrDefault(f => f.ActName.Equals(item.ActName, StringComparison.CurrentCultureIgnoreCase));
                act.Approver = item.ActorParms;
                act.ApproverName = item.ActorParmsName;
                actList.Add(act);
            }
            await this.CurrentDb.Updateable(actList).ExecuteCommandHasChangeAsync();
        }
    }
}
