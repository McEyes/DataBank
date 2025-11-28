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
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos
{
    /// <summary>
    /// 数据库表信息
    /// </summary>
    [Serializable]
    [ElasticIndexName("AssetClientScopes", "DataAsset")]
    [SugarTable("asset_client_scopes")]
    public class AssetClientScopesEntity : Entity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; set; }


        /// <summary>
        /// object id
        /// </summary>
        [SugarColumn(ColumnName = "object_id")]
        public string ObjectId { get; set; }

        /// <summary>
        /// object type
        /// table
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "object_type")]
        public string ObjectType { get; set; }

        /// <summary>
        /// topic id
        /// </summary>
        [SugarColumn(ColumnName = "ctl_id", Length = 50)]
        public string CtlId { get; set; }


        /// <summary>
        /// client uuid
        /// </summary>
        [SugarColumn(ColumnName = "client_uid")]
        public Guid? ClientUId { get; set; }

        /// <summary>
        /// client id
        /// </summary>
        [SugarColumn(ColumnName = "client_id", Length = 64)]
        public string ClientId { get; set; }

        /// <summary>
        /// client name
        /// </summary>
        [SugarColumn(ColumnName = "client_name", Length = 512)]
        public string ClientName { get; set; }

        /// <summary>
        /// table columns
        /// 授权的columns
        /// </summary>
        [SugarColumn(ColumnName = "table_columns", IsJson = true)]
        public List<DataColumnDto>? TableColumns { get; set; }

        /// <summary>
        /// 是否包含了全部字
        /// </summary>
        [SugarColumn(ColumnName = "is_all_columns")]
        public bool? IsAllColumns { get; set; }

        /// <summary>
        /// 是否包含了全部字
        /// </summary>
        [SugarColumn(ColumnName = "owner_ids", IsJson = true)]
        public string[]? OwnerIds { get; set; }


        [SugarColumn(ColumnName = "create_time", IsOnlyIgnoreUpdate = true)]
        public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// 密钥备注
        /// </summary>
        [SugarColumn(ColumnName = "description", Length = 512)]
        public string Description { get; set; }

        /// <summary>
        /// 流程编号
        /// </summary>
        [SugarColumn(ColumnName = "flow_no", Length = 64)]
        public string FlowNo { get; set; }
        /// <summary>
        /// 所属用户id
        /// </summary>
        [SugarColumn(ColumnName = "search_data")]
        public string SearchData { get; set; }
        /// <summary>
        /// 限制配置
        /// </summary>
        [SugarColumn(ColumnName = "use_rule", IsJson = true)]
        public ClientConfigRule ConfigRule { get; set; }

    }

    /// <summary>
    /// 使用规则：isMasking是否脱敏，limit:分页限制，"times": 3,"seconds": 60 , "enable": "true"
    /// </summary>
    public class ClientConfigRule
    {
        public bool IsMarking { get; set; }
        public int Limit { get; set; }
        public int Times { get; set; }
        public int Seconds { get; set; }
        public bool Enable { get; set; }
    }
}
