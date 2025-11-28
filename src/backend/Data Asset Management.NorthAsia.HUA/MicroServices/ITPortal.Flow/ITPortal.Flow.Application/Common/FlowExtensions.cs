using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Flow.Application.FlowActInst.Dtos;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowInst.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Core;
using ITPortal.Flow.Core.Enums;

using Newtonsoft.Json.Linq;

namespace ITPortal.Flow.Application.Common
{
    public class FlowExtensions
    {

        /// <summary>
        /// 获取开始节点，以Start名称开始
        /// </summary>
        /// <param name="actDtos"></param>
        /// <returns></returns>
        public static FlowActInstEntity GetStartAct(List<FlowActInstEntity> actDtos)
        {
            var startActName = ActType.Start.ToString();
            var currentAct = actDtos.FirstOrDefault(f => f.ActName.Equals(startActName, StringComparison.CurrentCultureIgnoreCase));
            if (currentAct == null)
                currentAct = actDtos.OrderBy(f => f.ActStep).FirstOrDefault();
            return currentAct;
        }

        /// <summary>
        /// 流程结束节点，以End名称结束
        /// </summary>
        /// <param name="actDtos"></param>
        /// <returns></returns>
        public static FlowActInstEntity GetEndAct(List<FlowActInstEntity> actDtos)
        {
            var startActName = ActType.End.ToString();
            var currentAct = actDtos.FirstOrDefault(f => f.ActName.Equals(startActName, StringComparison.CurrentCultureIgnoreCase));
            if (currentAct == null)
                currentAct = actDtos.OrderByDescending(f => f.ActStep).FirstOrDefault();
            return currentAct;
        }
        /// <summary>
        /// 获取下一个节点，默认开始节点的下一个节点
        /// </summary>
        /// <param name="actDtos"></param>
        /// <param name="currentActName"></param>
        /// <returns></returns>
        /// <exception cref="AppFriendlyException"></exception>
        public static FlowTempActDto GetStartNextAct(List<FlowTempActDto> actDtos, string currentActName = "Start")
        {
            var currentAct = actDtos.FirstOrDefault(f => f.ActName.Equals(currentActName, StringComparison.CurrentCultureIgnoreCase));
            FlowTempActDto nextAct = null;
            if (currentAct == null)
            {
                if (ActType.Start.ToString() == currentActName) nextAct = actDtos.FirstOrDefault(f => f.ActStep == 1);
                else throw new AppFriendlyException($"Process configuration exception, the current node {currentActName} does not exist", "5003");
            }
            nextAct = actDtos.FirstOrDefault(f => f.ActName == SwitchNextActName(currentAct, FlowAction.Submit));
            return nextAct;
        }

        public static FlowActInstEntity GetStartNextAct(FlowInstEntity flowInst, string currentActName = "Start")
        {
            var currentAct = flowInst.FlowActs.FirstOrDefault(f => f.ActName.Equals(currentActName, StringComparison.CurrentCultureIgnoreCase));
            FlowActInstEntity nextAct = null;
            if (currentAct == null)
            {
                if (ActType.Start.ToString() == currentActName) nextAct = flowInst.FlowActs.FirstOrDefault(f => f.ActStep == 1);
                else throw new AppFriendlyException($"Process configuration exception, the current node {currentActName} does not exist", "5003");
            }
            nextAct = flowInst.FlowActs.FirstOrDefault(f => f.ActName == SwitchNextActName(currentAct, FlowAction.Submit));
            return nextAct;
        }

        /// <summary>
        /// 获取下一个节点，根据审批动作和路径
        /// </summary>
        /// <param name="actDtos"></param>
        /// <param name="currentActName"></param>
        /// <param name="approvalAction"></param>
        /// <returns></returns>
        /// <exception cref="AppFriendlyException"></exception>
        public static string GetNextActName(List<FlowTempActDto> actDtos, string currentActName, FlowAction approvalAction)
        {
            var currentAct = actDtos.FirstOrDefault(f => f.ActName.Equals(currentActName, StringComparison.CurrentCultureIgnoreCase));
            FlowTempActDto nextAct = null;
            if (currentAct == null)
            {
                if (ActType.Start.ToString() == currentActName) nextAct = actDtos.FirstOrDefault(f => f.ActStep == 1);
                else throw new AppFriendlyException($"Process configuration exception, the current node {currentActName} does not exist", "5003");
            }
            return SwitchNextActName(currentAct, approvalAction);
        }

        public static string SwitchNextActName(FlowTempActDto currentAct, FlowAction approvalAction)
        {
            if (ActType.End.ToString().Equals(currentAct.ActName, StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }
            var nextAct = currentAct.SwitchPath.Where(f => f.Action.Equals(approvalAction.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (nextAct.Count != 1)
            {
                throw new AppFriendlyException($"Routing configuration exception! [{currentAct.ActName}] node {approvalAction.ToString()} action route is not unique, target route quantity: {nextAct.Count}.", "5004");
            }
            return nextAct.First().NextActInsName;
        }

        public static string SwitchNextActName(FlowActInstEntity currentAct, FlowAction approvalAction,JObject paramData=null)
        {
            if (ActType.End.ToString().Equals(currentAct.ActName, StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }
            var nextAct = currentAct.SwitchPath.Where(f => f.Action.Equals(approvalAction.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (nextAct.Count > 1)
            {
                //有参数计算表达式
                if (paramData != null)
                {
                    //检查是否存在表达式条件等
                    nextAct = nextAct.Where(f => f.RouteExecution.IsNullOrWhiteSpace() || ITPortal.Core.ExpressionTool.EvaluateBoolean(f.RouteExecution, paramData)).ToList();
                }
                if (nextAct.Count > 1)
                {
                    //满足条件的只有一个路由
                    //并签的时候，只要满足条件的节点都要返回，现在暂时不支持
                    return nextAct.OrderBy(f => f.Sort).First().NextActInsName;
                }
                else if (nextAct.Count > 0)
                {
                    return nextAct.OrderBy(f => f.Sort).First().NextActInsName;
                }
                else
                {
                    throw new AppFriendlyException($"Routing configuration exception! [{currentAct.ActName}] node {approvalAction.ToString()} action no matching routes, target route quantity: {nextAct.Count}.", "5005");
                }
            }
            else if (nextAct.Count == 1)
            {
                return nextAct.First().NextActInsName;
            }
            else
            {
                throw new AppFriendlyException($"Routing configuration exception! [{currentAct.ActName}] node {approvalAction.ToString()} action route is not unique, target route quantity: {nextAct.Count}.", "5004");
            }
        }

        public static async Task<Result> CallBack(BaseProxyService proxyService, FlowInstEntity flowInst, FlowActInstEntity currentAct, FlowAuditDto auditInfo, FlowAction approvalAction)
        {
            if (flowInst.CallBackUrl.IsNotNullOrWhiteSpace())
            {
                var flowDto = flowInst.Adapt<FlowInstDto>();
                flowDto.FlowActs = null;
                var actDto = currentAct.Adapt<FlowActInstDto>();
                actDto.SwitchPath = null;
                var callBackData = new Result<FlowBackDataEntity>()
                {
                    Data =new FlowBackDataEntity()
                    {
                        FlowInst = flowInst,
                        CurrentAct = actDto,
                        FlowAuditRecord = auditInfo,
                        ActionType = approvalAction
                    }
                };
                var url = ApiEmailHelper.FillTemplateByJson(flowInst.CallBackUrl, flowInst.FormData, FlowExtensions.GetFlowParamDict(flowInst));
                return await proxyService.httpRemoteService.PostAsAsync<Result<object>>(url,
                   builder => builder.SetContent(callBackData, "application/json;charset=utf-8")
                   .AddAuthentication("Bearer", proxyService.GetToken()));
            }
            return new Result();
        }


        public static Dictionary<string, object> GetFlowParamDict(FlowInstEntity flowInst, Dictionary<string, object> paramData = null)
        {
            var hostUrl = App.GetConfig<string>("RemoteApi:AppHostUrl");
            if (paramData == null) paramData = new Dictionary<string, object>();
            else if (paramData.ContainsKey("FlowId")) return paramData;
            paramData.Add("FlowId", flowInst.Id);
            paramData.Add("FlowNo", flowInst.FlowNo);
            paramData.Add("FlowTitle", flowInst.FlowTempTitle);
            paramData.Add("FlowStatus", flowInst.FlowStatus);
            paramData.Add("FlowTempId", flowInst.FlowTempId);
            paramData.Add("FlowTempName", flowInst.FlowTempName);
            paramData.Add("FlowType", flowInst.FlowTempName);

            paramData.Add("TaskSubject", flowInst.TaskSubject);
            paramData.Add("BasicUrl", hostUrl);

            if (flowInst.FormUrl.IsNotNullOrWhiteSpace())
            {
                if (flowInst.FormUrl.IndexOf("?") > 0)
                    paramData.Add("FullFormUrl", $"{hostUrl}{flowInst.FormUrl}&flowTempName={flowInst.FlowTempName}&id={flowInst.Id}");
                else
                    paramData.Add("FullFormUrl", $"{hostUrl}{flowInst.FormUrl}?flowTempName={flowInst.FlowTempName}&id={flowInst.Id}");
            }
            else
            {
                paramData.Add("FullFormUrl", $"{hostUrl}/#/home/workflow?flowTempName={flowInst.FlowTempName}&id={flowInst.Id}");
            }
            paramData.Add("FormUrl", flowInst.FormUrl);

            paramData.Add("CurrentActStep", flowInst.FlowStep);
            paramData.Add("CurrentActName", flowInst.FlowStepName);
            paramData.Add("CurrentActTitle", flowInst.FlowStepTitle);

            paramData.Add("Applicant", flowInst.Applicant);
            paramData.Add("ApplicantName", flowInst.ApplicantName);
            paramData.Add("ApplicantEmail", flowInst.ApplicantEmail);

            paramData.Add("CurrentApproverName", flowInst.ApproverName);

            paramData.Add("CreateBy", flowInst.CreateBy);
            paramData.Add("CreateByName", flowInst.CreatedByName);

            paramData.Add("CompleteDate", flowInst.CompleteTime?.ToString());
            paramData.Add("CompleteDateTime", flowInst.CompleteTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            paramData.Add("CompleteTime", flowInst.CompleteTime?.ToString("HH:mm:ss"));

            paramData.Add("CreateDate", flowInst.CreateTime.ToString());
            paramData.Add("CreateDateTime", flowInst.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            paramData.Add("CreateTime", flowInst.CreateTime.ToString("HH:mm:ss"));
            return paramData;
        }

        public class FlowBackDataEntity
        {
            public FlowInstEntity FlowInst { get; set; }
            public FlowActInstDto CurrentAct { get; set; }
            public FlowAuditDto FlowAuditRecord { get; set; }
            public FlowAction ActionType { get; set; }
        }
    }
}
