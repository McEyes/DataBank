using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.AssetClients.Dtos
{
    /// <summary>
    /// 新api申请表单
    /// 单表申请，支持表单字段安全等级授权
    /// </summary>
    [Serializable]
    public class ApplyAuthApplyDto
    {
        /// <summary>
        /// 申请类型：1 Individual个人,2 Application应用程序
        /// </summary>
        public int ApplyType { get; set; } = 1;
        /// <summary>
        /// 涉密等级，默认=1公共及别，不需要审批，
        /// 根据申请表格和表格字段的最大等级
        /// </summary>
        public int SecurityLevel { get; set; } = 1;

        /// <summary>
        /// 2 Application应用程序时的appId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 2 Application应用程序时的AppName
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 申请人id
        /// </summary>
        [Description("申请人id")]
        [Required(ErrorMessage = "申请人不能为空")]

        public string UserId { get; set; }

        [Description("申请人名称")]
        public string UserName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 表单编号
        /// </summary>
        [Description("表单编号")]
        public string ApplyFormNo { get; set; }

        /// <summary>
        /// 申请目的
        /// </summary>
        [Description("申请目的")]
        public string Reason { get; set; }


        /// <summary>
        /// 有效期类型：999永久有效，12一年，6半年，3三个月，1一个月
        /// </summary>
        [Description("有效期")]

        public int? PeriodType { get; set; } //截止日期： 非必填

        /// <summary>
        /// 有效期日期：添加999个月，
        /// 截止日期
        /// </summary>
        [Description("有效期日期")]
        public DateTimeOffset? ValidityPeriodDate { get; set; } //截止日期： 非必填


        public List<ApplyTableInfo> TableList { get; set; } //选择的表的信息
    }
}
