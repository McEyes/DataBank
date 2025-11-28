using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

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

namespace DataAssetManager.DataApiServer.Application.DataTable.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("asset_doc")]
    public class AssetDocEntity : AuditEntity<Guid>
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "extension")]
        public string Extension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "ver")]
        public int? DocVer { get; set; }
        /// <summary>
        /// approver
        /// </summary>
        [SugarColumn(ColumnName = "display_name_cn")]
        public string DisplayNameCn { get; set; }
        /// <summary>
        /// approver
        /// </summary>
        [SugarColumn(ColumnName = "display_name_en")]
        public string DisplayNameEn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "sort")]
        public int? Sort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public bool? Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "catalog")]
        public string Catalog { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "doc_icon_url")]
        public string DocIconUrl { get; set; }

    }

}
