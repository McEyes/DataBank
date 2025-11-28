using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataAuthApplyDto : PageEntity<Guid>
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        public string? FlowNo { get; set; }


        /// <summary>
        /// 申请人ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string UserName { get; set; }
        ///// <summary>
        ///// 申请人姓名
        ///// </summary>
        //public string UserDept { get; set; }

        /// <summary>
        /// 申请人SME
        /// </summary>
        public string? SmeId { get; set; }
        /// <summary>
        /// 申请人SME名称
        /// </summary>
        public string? SmeName { get; set; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string SmeDept { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// 当前审批节点
        /// </summary>
        public string? ApplyNode { get; set; }


        /// <summary>
        /// 流程完成时间
        /// </summary>
        public DateTimeOffset? CompletionTime { get; set; }


        /// <summary>
        /// DataCatalog id
        /// </summary>
        public string? CtlId { get; set; }
        /// <summary>
        /// DataCatalog id
        /// </summary>
        public string? CtlName { get; set; }
        /// <summary>
        /// DataCatalog id
        /// </summary>
        public string? CtlRemark { get; set; }
        /// <summary>
        /// 申请类型：1个人，2app
        /// </summary>
        public string? ApplyType { get; set; }
        /// <summary>
        /// app id
        /// </summary>
        public string? AppId { get; set; }
        /// <summary>
        /// app name
        /// </summary>
        public string? AppName { get; set; }
        /// <summary>
        /// app owner name
        /// </summary>
        public string? AppOwner { get; set; }

        /// <summary>
        /// 涉密级别
        /// </summary>
        public string? LevelId { get; set; }
    }

}
