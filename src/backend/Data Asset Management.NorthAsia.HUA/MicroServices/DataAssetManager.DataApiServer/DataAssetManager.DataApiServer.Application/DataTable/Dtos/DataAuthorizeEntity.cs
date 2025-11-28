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
    [ElasticIndexName("DataAuthorize", "DataAsset")]
    [SugarTable("metadata_authorize")]
    public class DataAuthorizeEntity : Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "object_id")]
        public string ObjectId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "object_type")]
        public string ObjectType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "role_id")]
        public string RoleId { get; set; }


    }

}
