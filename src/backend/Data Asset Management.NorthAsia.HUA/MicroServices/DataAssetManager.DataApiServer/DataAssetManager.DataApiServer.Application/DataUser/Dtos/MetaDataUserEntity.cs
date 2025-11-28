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
    [ElasticIndexName("MetaDataUser", "DataAsset")]
    [SugarTable("metadata_user")]
    public class MetaDataUserEntity : Entity<string>
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
        [SugarColumn(ColumnName = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "user_name")]
        public string UserName { get; set; }
        /// <summary>
        /// approver
        /// </summary>
        [SugarColumn(ColumnName = "user_type")]
        public string UserType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "sort")]
        public int Sort { get; set; }

    }

}
