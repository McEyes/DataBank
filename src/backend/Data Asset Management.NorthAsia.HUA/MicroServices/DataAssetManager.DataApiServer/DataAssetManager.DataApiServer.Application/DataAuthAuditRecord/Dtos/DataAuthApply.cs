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
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataAuthApply
    {
        /// <summary>
        /// 申请人id
        /// </summary>
        [Description("申请人id")]
        [Required(ErrorMessage = "申请人不能为空")]

        public string UserId;

        [Description("申请人名称")]
        public string UserName;

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark;

        /// <summary>
        /// 申请id入参
        /// </summary>
        [Description("申请id入参")]
        public string ApplyFormId;

        /// <summary>
        /// 申请目的
        /// </summary>
        [Description("申请目的")]
        public string Reason;

        /// <summary>
        /// 截止日期
        /// </summary>
        [Description("截止日期")]

        public DateTimeOffset? RequireDate; //截止日期： 非必填


        public List<DataTableInfo> TableList; //选择的表的信息
    }
}
