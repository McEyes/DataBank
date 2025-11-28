using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{


    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("metadata_table")]
    public class MetadataTableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? source_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? table_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? table_comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? create_by { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? create_time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? create_org { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? update_by { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? update_time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? db_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? is_sync { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? db_schema { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? reviewable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? alias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? jsonsql_config { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? update_frequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? update_method { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? data_time_range { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? owner_depart { get; set; }

        /// <summary>
        /// 质量分数
        /// </summary>
        public int? quality_score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? data_category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? last_score { get; set; }

        /// <summary>
        /// 更新类型：Full全量，Incremental增量
        /// </summary>
        public string? update_category { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public string? table_code { get; set; }

    }
}
