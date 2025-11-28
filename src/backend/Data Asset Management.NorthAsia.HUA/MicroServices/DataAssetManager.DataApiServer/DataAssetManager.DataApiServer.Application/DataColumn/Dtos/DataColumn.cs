using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPortal.Core.Extensions;

namespace DataAssetManager.DataApiServer.Application.DataColumn.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataColumnInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableId { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string ColComment { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string ColKey { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string Nullable { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public int? ColPosition { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string DataLength { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string DataPrecision { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string DataScale { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string DataDefault { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public bool Sortable { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public bool RequiredAsCondition { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string MasterdataType { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public bool Standardized { get; set; }
        /// <summary>
        /// 安全级别
        /// </summary>
        public string LevelId { get; set; } = "2";
        /// <summary>
        /// indicator code
        /// 指标code
        /// </summary>
        public string IndicatorCode { get; set; }


        /// <summary>
        /// 质量分数
        /// </summary>
        public decimal QualityScore { get; set; }
        /// <summary>
        /// 上一次质量分数
        /// </summary>
        public decimal LastScore { get; set; }

        /// <summary>
        /// 安全级别
        /// </summary>
        public string LevelName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LevelId)) return "";
                return LevelId.GetEnum<SecurityLevel>().GetDescription();
            }
        }

        /// <summary>
        /// 表的级别有column的级别判断
        /// </summary>
        public bool IsPublicSecurityLevel
        {
            get
            {
                return LevelId == "1" || LevelId == "2" || LevelId == "" || LevelId == null;
            }
        }
    }

}
