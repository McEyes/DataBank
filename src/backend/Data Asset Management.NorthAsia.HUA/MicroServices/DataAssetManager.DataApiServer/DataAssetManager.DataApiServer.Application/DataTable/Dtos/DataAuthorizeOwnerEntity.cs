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
    [ElasticIndexName("DataAuthorizeOwner", "DataAsset")]
    [SugarTable("metadata_authorize_owner")]
    public class DataAuthorizeOwnerEntity : Entity<string>
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
        [SugarColumn(ColumnName = "owner_id")]
        public string OwnerId { get; set; }

        /// <summary>
        /// 3机密,2	内部使用级,1	公众级,4	受限机密
        /// </summary>
        [SugarColumn(ColumnName = "owner_name")]
        public string OwnerName { get; set; }


        /// <summary>
        /// 拥有者所属部门
        /// </summary>
        [SugarColumn(ColumnName = "owner_dept")]
        public string OwnerDept { get; set; }
    }

}
