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

namespace DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [ElasticIndexName("AssetClientSecrets", "DataAsset")]
    [SugarTable("asset_client_secrets")]
    public class AssetClientSecretsEntity : Entity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        /// <summary>
        /// secret type
        /// 个人还是app
        /// </summary>
        [SugarColumn(ColumnName = "type")]
        public string Type { get; set; }

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
        /// 密钥备注
        /// </summary>
        [SugarColumn(ColumnName = "description", Length = 512)]
        public string Description { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [SugarColumn(ColumnName = "expiration")]
        public DateTimeOffset? Expiration { get; set; }
        /// <summary>
        /// 拥有人
        /// </summary>
        [SugarColumn(ColumnName = "secrets")]
        public string? Secrets { get; set; }

        /// <summary>
        /// 流程编号
        /// </summary>
        [SugarColumn(ColumnName = "flow_no", Length = 64)]
        public string FlowNo { get; set; }

        /// <summary>
        /// 所属用户id
        /// </summary>
        [SugarColumn(ColumnName = "owner_id", Length = 64)]
        public string OwnerId { get; set; }

        /// <summary>
        /// createTime
        /// </summary>
        [SugarColumn(ColumnName = "create_time")]
        public virtual DateTimeOffset CreateTime { get; set; }

    }
}
