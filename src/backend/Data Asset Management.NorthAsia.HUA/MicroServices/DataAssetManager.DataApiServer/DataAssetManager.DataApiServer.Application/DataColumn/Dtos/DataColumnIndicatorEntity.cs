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
using ITPortal.Core.LightElasticSearch;
using Microsoft.OpenApi.Extensions;
using ITPortal.Core.Extensions;

namespace DataAssetManager.DataApiServer.Application.DataColumn.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("metadata_indicator_score_view")]
    public class DataColumnIndicatorEntity : Entity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "source_id")]
        public string SourceId { get; set; }
        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "source_code")]
        public string SourceCode { get; set; }
        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "source_name")]
        public string SourceName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "table_id")]
        public string TableId { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "table_code")]
        public string TableCode { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "table_name")]
        public string TableName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "table_comment")]
        public string TableComment { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public string TableStatus { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "alias")]
        public string TableAlias { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "update_frequency")]
        public string UpdateFrequency { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "owner_id")]
        public string OwnerId { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "owner_name")]
        public string OwnerName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "owner_depart")]
        public string OwnerDept { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "owner_depart_code")]
        public string OwnerDeptCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "level_id")]
        public string? TableLevelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "level_name")]
        public string? TableLevelName { get; set; }

        /// <summary>
        /// topic ID
        /// </summary>
        [SugarColumn(ColumnName = "ctl_id")]
        public string CtlId { get; set; }

        /// <summary>
        /// topic名称
        /// </summary>
        [SugarColumn(ColumnName = "ctl_name")]
        public string CtlName { get; set; }

        /// <summary>
        /// topic代码
        /// </summary>
        [SugarColumn(ColumnName = "ctl_code")]
        public string CtlCode { get; set; }
        /// <summary>
        /// topic代码
        /// </summary>
        [SugarColumn(ColumnName = "ctl_remark")]
        public string CtlRemark { get; set; }
        /// <summary>
        /// topic代码
        /// </summary>
        [SugarColumn(ColumnName = "ctl_status")]
        public string CtlStatus { get; set; }




        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "column_id")]
        public string ColId { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "column_code")]
        public string ColCode { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "column_name")]
        public string ColName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "column_comment")]
        public string ColComment { get; set; }

        /// <summary>
        /// 安全级别
        /// </summary>
        [SugarColumn(ColumnName = "security_level")]
        public string LevelId { get; set; }



        /// <summary>
        /// indicator code
        /// 指标code
        /// </summary>
        [SugarColumn(ColumnName = "indicator_code", Length = 50)]
        public string IndicatorCode { get; set; }


        /// <summary>
        /// 质量分数
        /// </summary>
        [SugarColumn(ColumnName = "quality_score",IsNullable = true)]
        public decimal QualityScore { get; set; }
        /// <summary>
        /// 上一次质量分数
        /// </summary>
        [SugarColumn(ColumnName = "last_score")]
        public decimal LastScore { get; set; }
        /// <summary>
        /// 上一次质量分数
        /// </summary>
        [SugarColumn(ColumnName = "isindicator")]
        public bool IsIndicator { get; set; }
        /// <summary>
        /// 上一次质量分数
        /// </summary>
        [SugarColumn(ColumnName = "isdashboard")]
        public bool IsDashboard { get; set; }


        /// <summary>
        /// 安全级别
        /// </summary>
        [SugarColumn(IsIgnore = true)]
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
        [SugarColumn(IsIgnore = true)]
        public bool IsPublicSecurityLevel
        {
            get
            {
                return LevelId == "1" || LevelId == "2" || LevelId == "" || LevelId == null;
            }
        }
    }

}
