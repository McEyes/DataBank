using DataAssetManager.DataApiServer.Application.AssetClients.Dtos;

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

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 流程申请表单信息
    /// </summary>
    [Serializable]
    public class DataAuthApplyInfo
    {
        /// <summary>
        /// 申请类型：1个人，2app
        /// </summary>
        [Description("申请类型")]
        [Required(ErrorMessage = "申请类型不能为空")]
        public AuthApplyType ApplyType { get; set; }
        /// <summary>
        /// app id
        /// </summary>
        [Description(description: "App类型appid")]
        public string? AppId { get; set; }
        /// <summary>
        /// app name
        /// </summary>
        public string? AppName { get; set; }
        ///// <summary>
        ///// app owner name
        ///// </summary>
        //public string? AppOwner { get; set; }
        /// <summary>
        /// 申请人id
        /// </summary>
        [Description("申请人id")]
        [Required(ErrorMessage = "申请人不能为空")]

        public string UserId { get; set; }

        [Description("申请人名称")]
        public string UserName { get; set; }

        ///// <summary>
        ///// api的拥有者id，对应申请表单中的ownerid
        ///// </summary>
        //public string? ResourceOwnerId { get; set; }

        ///// <summary>
        ///// api的拥有者名称，对应申请表单中的ownername
        ///// </summary>
        //public string? ResourceOwnerName { get; set; }

        ///// <summary>
        ///// api的拥有者所属部门，对应申请表单中的ownerdept
        ///// </summary>
        //public string? ResourceOwnerDept { get; set; }

        ///// <summary>
        ///// 主题域id
        ///// </summary>
        //public string? CtlId { get; set; }
        ///// <summary>
        ///// 主题域名称
        ///// </summary>
        //public string? CtlName { get; set; }
        ///// <summary>
        ///// 主题域备注说明
        ///// </summary>
        //public string? CtlRemark { get; set; }


        ///// <summary>
        ///// 备注
        ///// </summary>
        //[Description("备注")]
        //public string? Remark { get; set; }

        /// <summary>
        /// 流程id，修改的时候必填
        /// </summary>
        [Description("流程id")]
        public Guid? ApplyId { get; set; }

        /// <summary>
        /// 申请目的
        /// </summary>
        [Required(ErrorMessage = "申请目的不能为空")]
        [Description("申请目的")]
        public string Reason { get; set; }

        ///// <summary>
        ///// 截止日期
        ///// </summary>
        //[Description("截止日期")]

        //public DateTimeOffset? RequireDate; //截止日期： 非必填


        [Required(ErrorMessage = "申请表不能为空")]
        [Description("申请表明细")]
        public List<DataAuthTableInfo> TableList { get; set; } //选择的表的信息
    }
}
