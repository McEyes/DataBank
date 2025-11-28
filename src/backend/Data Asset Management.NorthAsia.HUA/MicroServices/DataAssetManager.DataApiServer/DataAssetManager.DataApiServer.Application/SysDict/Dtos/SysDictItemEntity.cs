using ITPortal.Core;
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
using System.ComponentModel;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据API信息表 实体DTO
    /// </summary>
    [Serializable]
    [ElasticIndexName("SysDictItem", "DataAsset")]
    [SugarTable("asset_dict_item")]
    public class SysDictItemEntity : AuditEntity<string>
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override string Id { get; set; }

        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        [SugarColumn(ColumnName = "remark")]
        public string Remark { get; set; }

        [SugarColumn(ColumnName = "dict_id")]
        public string DictId { get; set; }

        [SugarColumn(ColumnName = "item_text")]
        public string ItemText { get; set; }

        [SugarColumn(ColumnName = "item_text_en")]
        public string ItemTextEn { get; set; }

        [SugarColumn(ColumnName = "item_value")]
        public string ItemValue { get; set; }

        [SugarColumn(ColumnName = "item_data")]
        public string ItemData { get; set; }

        [SugarColumn(ColumnName = "item_sort")]
        public int? ItemSort { get; set; }

    }
}
