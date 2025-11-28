using ITPortal.Core.DataSource;
using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using SqlSugar.Extensions;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据源信息表
    /// </summary>
    [Serializable]
    [ElasticIndexName("DataSource", "DataAsset")]
    [SugarTable("metadata_source")]
    public class DataSourceEntity : AuditEntity<string>
    {

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override string Id { get; set; }
        /// <summary>
        /// 数据源类型
        /// </summary>
        [SugarColumn(ColumnName = "db_type")]
        public int? DbType
        {
            get { return _DbType; }
            set
            {
                if (_DbType != value)
                {
                    _DbType = value;
                    if (this.DbSchema != null)
                        this.DbSchema.Dbtype = value;
                }
            }
        }
        private int? _DbType;

        /// <summary>
        /// 数据源名称
        /// </summary>
        [SugarColumn(ColumnName = "source_name")]
        public string SourceName { get; set; }

        /// <summary>
        /// 元数据同步（0否，1同步中, 2是 3-出错）
        /// </summary>
        [SugarColumn(ColumnName = "is_sync")]
        public string IsSync { get; set; }

        /// <summary>
        /// 数据源连接信息
        /// </summary>
        [SugarColumn(ColumnName = "db_schema", IsJson = true)]
        public DbSchema DbSchema
        {
            get { return _DbSchema; }
            set
            {
                if (value != _DbSchema)
                {
                    _DbSchema = value;
                    if (_DbSchema != null) _DbSchema.Dbtype = this.DbType;
                }
            }
        }
        private DbSchema _DbSchema;
        /// <summary>
        /// 数据源连接信息
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        /// <summary>
        /// 数据源连接信息
        /// </summary>
        [SugarColumn(ColumnName = "remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        [SugarColumn(ColumnName = "system_name")]
        public string SystemName { get; set; }
        /// <summary>
        /// 子表信息（不映射到数据库）
        /// </summary>
        [JsonIgnore]
        [SugarColumn(IsIgnore = true)]
        public List<DataTableEntity> Children { get; set; }

    }
}