using ITPortal.Core.Services;


namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("dashbord_metadata_record")]
    public class DashbordMetadataEntity : AuditEntity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        /// <summary>
        /// 数据源主数据ID
        /// </summary>
        [SugarColumn(ColumnName = "source_id")]
        public string SourceId { get; set; }

        /// <summary>
        /// 拥有者NTID
        /// </summary>
        [SugarColumn(ColumnName = "owner_id")]
        public string OwnerId { get; set; }

        /// <summary>
        ///是否是Indicator
        /// </summary>
        [SugarColumn(ColumnName = "isindicator")]
        public bool? IsIndicator { get; set; }



        [SugarColumn(ColumnName = "indicator_code")]
        public string IndicatorCode { get; set; }
        /// <summary>
        ///是否是Dashboard
        /// </summary>
        [SugarColumn(ColumnName = "isdashboard")]
        public bool? IsDashboard { get; set; }


        /// <summary>
        ///TableName
        /// </summary>
        [SugarColumn(ColumnName = "table_name")]
        public string TableName { get; set; }


        /// <summary>
        ///ColumnName
        /// </summary>
        [SugarColumn(ColumnName = "column_name")]
        public string ColumnName { get; set; }

        /// <summary>
        ///ColumnId
        /// </summary>
        [SugarColumn(ColumnName = "column_id")]
        public string ColumnId { get; set; }


    }
}
