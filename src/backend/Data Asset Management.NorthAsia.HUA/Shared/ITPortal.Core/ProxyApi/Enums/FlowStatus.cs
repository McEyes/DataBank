using System.ComponentModel;

namespace ITPortal.Core.ProxyApi.Flow.Enums
{

    /// <summary>
    /// draft
    /// running
    /// Stop
    /// Turn to do
    /// unusual
    /// </summary>
    public enum FlowStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 0,
        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Running = 1,
        ///// <summary>
        ///// 转办运行中
        ///// </summary>
        //[Description("转办")]
        //Transfer = 2,

        /// <summary>
        /// 取消，申请人取消，废弃状态，流程结束
        /// </summary>
        [Description("取消")]
        Cancel = -1,
        /// <summary>
        /// 异常终止
        /// </summary>
        [Description("异常终止")]
        Stop = -99,
        /// <summary>
        /// 正常结束
        /// </summary>
        [Description("完成")]
        Completed = 99,
    }

    /// <summary>
    /// draft
    /// running
    /// Stop
    /// Turn to do
    /// unusual
    /// 活动状态
    /// </summary>
    public enum ActivityStatus
    {
        /// <summary>
        /// 未到达
        /// </summary>
        [Description("")]
        None = 0,
        /// <summary>
        /// 已到达待审批
        /// </summary>
        [Description("待审批")]
        Running = 1,
        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已审批")]
        Approval = 2,
        /// <summary>
        /// 已经转办
        /// </summary>
        [Description("已转办")]
        Transfer = 3,
        /// <summary>
        /// 拒绝，驳回
        /// </summary>
        [Description("已驳回")]
        Reject = 5,
        /// <summary>
        /// 已取消，申请人取消
        /// </summary>
        [Description("已取消")]
        Cancel = -1,
        /// <summary>
        /// 驳回到开始
        /// </summary>
        [Description("已驳回到开始")]
        RejectStart = 90,
        /// <summary>
        /// 异常终止
        /// </summary>
        [Description("异常终止")]
        Stop = -99,
    }

    /// <summary>
    /// draft
    /// running
    /// Stop
    /// Turn to do
    /// unusual
    /// </summary>
    public enum FlowAction
    {
        /// <summary>
        /// 启动
        /// </summary>
        [Description("启动")]
        Start = 0,
        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        Submit = 1,
        /// <summary>
        /// 审批
        /// </summary>
        [Description("审批")]
        Approval = 2,
        /// <summary>
        /// 转办运行中
        /// </summary>
        [Description("转办")]
        Transfer = 3,
        /// <summary>
        /// 拒绝，驳回
        /// </summary>
        [Description("驳回")]
        Reject = 5,
        /// <summary>
        /// RejectStart
        /// </summary>
        [Description("驳回到开始")]
        RejectStart = 90,
        /// <summary>
        /// RejectEnd，流程结束
        /// </summary>
        [Description("拒绝")]
        RejectEnd = 99,
        /// <summary>
        /// Cancel
        /// </summary>
        [Description("取消")]
        Cancel = -1,
        /// <summary>
        /// 正常结束
        /// </summary>
        [Description("结束")]
        End = 4,
    }
    /// <summary>
    /// draft
    /// running
    /// Stop
    /// Turn to do
    /// unusual
    /// </summary>
    public enum TodoStatus
    {
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Todo = 0,
        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已审批")]
        Audit = 2,
        /// <summary>
        /// 已转办
        /// </summary>
        [Description("已转办")]
        Transfer = 3,
        /// <summary>
        /// 已驳回
        /// </summary>
        [Description("已驳回")]
        Reject = 5,
        /// <summary>
        /// 已取消，申请人取消
        /// </summary>
        [Description("已取消")]
        Cancel = -1,
        /// <summary>
        /// 已驳回到开始节点
        /// </summary>
        [Description("已驳回")]
        RejectStart = 90,
        /// <summary>
        /// 异常停止
        /// </summary>
        [Description("异常停止")]
        Error = -99,
        ///// <summary>
        ///// 结束
        ///// </summary>
        //[Description("结束")]
        //End = 99,
    }

    /// <summary>
    /// draft
    /// running
    /// Stop
    /// Turn to do
    /// unusual
    /// </summary>
    public enum FlowNoticeType
    {
        /// <summary>
        /// Todo 待办
        /// </summary>
        [Description("Todo")]
        Todo = 0,
        /// <summary>
        /// Email邮件
        /// </summary>
        [Description("Email")]
        Email = 1,
        /// <summary>
        /// 短信SMS
        /// </summary>
        [Description("SMS")]
        SMS = 2,
        /// <summary>
        /// Mobile移动端
        /// </summary>
        [Description("Mobile")]
        Mobile = 4,
    }
}
