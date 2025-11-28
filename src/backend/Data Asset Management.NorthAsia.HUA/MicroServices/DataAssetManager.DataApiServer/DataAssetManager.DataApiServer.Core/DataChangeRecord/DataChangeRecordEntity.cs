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

namespace DataAssetManager.DataApiServer.Core
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [ElasticIndexName("DataChangeRecord", "DataAsset")]
    [SugarTable("metadata_change_record")]
    public class DataChangeRecordEntity : CreateEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "remark", Length = 255)]
        public string Remark { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "version", Length = 255, IsNullable = true)]
        public int? Version { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "object_type", Length = 255, IsNullable = true)]
        public string ObjectType { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "object_id", Length = 255, IsNullable = true)]
        public string ObjectId { get; set; }


        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "field_name", Length = 255, IsNullable = true)]
        public string FieldName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "field_old_value", Length = 255, IsNullable = true)]
        public string FieldOldValue { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "field_new_value", Length = 255, IsNullable = true)]
        public string FieldNewValue { get; set; }

    }
}
