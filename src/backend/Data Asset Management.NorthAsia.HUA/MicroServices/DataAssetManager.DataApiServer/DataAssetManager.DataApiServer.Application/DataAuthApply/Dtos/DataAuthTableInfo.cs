using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

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
    /// 授权申请表明细申请表信息
    /// </summary>
    [Serializable]
    public class DataAuthTableInfo
    {

        /// <summary>
        /// 表Id
        /// </summary>
        public string Id { get { return TableId; } set { TableId = value; } }

        /// <summary>
        /// 主题域ID
        /// </summary>
        [Description("主题域ID")]
        public string CtlId { get; set; }

        /// <summary>
        /// 主题域名称
        /// </summary>
        [Description("主题域名称")]
        public string CtlName { get; set; }

        /// <summary>
        /// 主题域Code
        /// </summary>
        public string CtlCode { get; set; }

        /// <summary>
        /// 主题域备注
        /// </summary>
        public string CtlRemark { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 表Id
        /// </summary>
        public string TableId { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string TableComment { get; set; }
        /// <summary>
        /// 表别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 表所有者ID
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// 表所有者名称
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 表所有者所在部门
        /// </summary>
        public string OwnerDept { get; set; }


        /// <summary>
        /// 字段涉密级别
        /// </summary>
        public string LevelId { get; set; }

        /// <summary>
        /// 授权字段
        /// </summary>
        public List<DataColumnInfo> ColumnList { get; set; }

        public bool IsPublicSecurityLevel
        {
            get
            {
                return LevelId == "1" || LevelId == "2";
            }
        }

        public int? NeedSup { get; internal set; }
        public string Data { get; internal set; }
        public bool? AllColumns { get; set; }
    }
}
