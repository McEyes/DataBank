using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.ProxyApi.Dto
{

    public class FlowDataGrantInfo
    {
        public string UserId { get; set; }
        public string SMEId { get; set; }
        public string SMEEmail { get; set; }
        public string? SMEName { get; set; }
        /// <summary>
        /// 暂存人员名称,后端获取
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 申请表名
        /// </summary>
        public string[] Tables { get; set; }
        /// <summary>
        /// 需要审批完成时间
        /// </summary>
        public DateTimeOffset? RequireDate { get; set; }
        public string? ApplyFormNo { get; set; }
        public string ApplyFormId { get; set; }
        public string Remark { get; set; }




        /// <summary>
        /// 目的
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 节点
        /// </summary>
        public List<ApplyFlowCommonNodeInput> NodeList { get; set; }
    }

    public class ApplyFlowCommonNodeInput
    {
        /// <summary>
        /// 是否有角色(部门)
        /// </summary>
        public bool HasRole { get; set; }
        /// <summary>
        /// 通过类型:1,会签;2,并签;
        /// </summary>
        public int PassType { get; set; }

        /// <summary>
        /// 通过之后单据状态;
        /// </summary>
        public string? PassStatus { get; set; }

        /// <summary>
        /// 节点序号
        /// </summary>
        public int NodeIndex { get; set; }
        /// <summary>
        /// 0代表在当前节点,99代表结束;-1到起点;
        /// </summary>
        public int RejectIndex { get; set; }

        /// <summary>
        /// 0代表在当前节点,99代表结束;-1到起点,可不填
        /// </summary>
        public int? ReturnIndex { get; set; }

        /// <summary>
        /// 是否要发送ToDo
        /// </summary>
        public bool IsSendTodo { get; set; }
        /// <summary>
        /// 是否要发送邮件
        /// </summary>
        public bool IsSendMail { get; set; }
        public string? NodeDisplayName { get; set; }
        public List<ApplyFlowApproverItemInput> ApproverList { get; set; }
    }

    public class ApplyFlowApproverItemInput
    {
        public string? RoleName { get; set; }
        public string UserId { get; set; }
    }
}
