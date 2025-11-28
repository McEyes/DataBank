using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.ApproveFlow.Enums
{
    public enum BMFlowAction
    {
        /// </summary>
        [Description("审批")]
        Approval = 2,
        /// <summary>
        /// 拒绝，驳回
        /// </summary>
        [Description("驳回")]
        Reject = 5,
        /// <summary>
        /// RejectEnd，流程结束
        /// </summary>
        [Description("拒绝")]
        RejectEnd = 99
    }
}
