using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{

    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("metadata_column")]
    public class MetadataColumnEntity
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
        public string? table_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? column_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? column_comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? column_key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? column_nullable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? column_position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? data_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? data_length { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? data_precision { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? data_scale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? data_default { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? sortable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? required_as_condition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? standardized { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? masterdata_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? security_level { get; set; }

        /// <summary>
        /// 字段编码
        /// </summary>
        public string? column_code { get; set; }

        /// <summary>
        /// Indicator code
        /// </summary>
        public string? indicator_code { get; set; }

        /// <summary>
        /// indicator quality score, 指标质量分数
        /// </summary>
        public decimal? quality_score { get; set; }

        /// <summary>
        /// indicator 上一次质量分数
        /// </summary>
        public decimal? last_score { get; set; }





    }
}
