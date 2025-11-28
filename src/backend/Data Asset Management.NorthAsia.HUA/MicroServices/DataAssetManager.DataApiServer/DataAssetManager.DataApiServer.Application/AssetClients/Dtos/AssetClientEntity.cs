using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [ElasticIndexName("AssetClient", "DataAsset")]
    [SugarTable("asset_clients")]
    public class AssetClientEntity : AuditEntity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        /// <summary>
        /// 申请类型：1 Individual个人,2 Application应用程序
        /// </summary>
        [SugarColumn(ColumnName = "client_type")]
        public int ClientType { get; set; }

        /// <summary>
        /// 应用ID,也是访问key
        /// </summary>
        [SugarColumn(ColumnName = "client_id", Length = 64)]
        public string ClientId { get; set; }
        /// <summary>
        ///client name
        ///应用名称
        /// </summary>
        [SugarColumn(ColumnName = "client_name", Length = 128)]
        public string ClientName { get; set; }
        /// <summary>
        ///缩写
        /// </summary>
        [SugarColumn(ColumnName = "nick_name", Length = 128)]
        public string NickName { get; set; }
        /// <summary>
        ///链接
        /// </summary>
        [SugarColumn(ColumnName = "client_url", Length = 1024)]
        public string ClientUrl { get; set; }
        /// <summary>
        /// 应用简介
        /// </summary>
        [SugarColumn(ColumnName = "description", Length = 256)]
        public string Description { get; set; }

        /// <summary>
        /// 是否有效:0,1
        /// </summary>
        [SugarColumn(ColumnName = "enabled")]
        public bool? Enabled { get; set; }
        /// <summary>
        /// 拥有人
        /// </summary>
        [SugarColumn(ColumnName = "owner_id")]
        public string? Owner { get; set; }
        /// <summary>
        /// 拥有人姓名
        /// </summary>
        [SugarColumn(ColumnName = "owner_name")]
        public string? OwnerName { get; set; }

        /// <summary>
        /// 拥有人部门
        /// </summary>
        [SugarColumn(ColumnName = "owner_dept")]
        public string? OwnerDept { get; set; }

        /// <summary>
        /// 拥有人部门
        /// </summary>
        [SugarColumn(ColumnName = "owner_ntid")]
        public string? OwnerNtid { get; set; }
        /// <summary>
        /// 访问人IP白名单
        /// </summary>
        [SugarColumn(ColumnName = "whiteip_list")]
        public string? WhiteipList { get; set; }
        /// <summary>
        /// sme_list
        /// </summary>
        [SugarColumn(ColumnName = "sme_list", IsJson = true)]
        public List<StaffUser>? SMEList { get; set; } = new List<StaffUser>();
        /// <summary>
        /// 类别（Global/Regional/Site）
        /// </summary>
        [SugarColumn(ColumnName = "belong_area")]
        public string? BelongArea { get; set; }
        /// <summary>
        /// 主数据类型（多选）Json array
        /// </summary>
        [SugarColumn(ColumnName = "master_data_types", IsJson = true)]
        public List<string>? MasterDataTypes { get; set; }=new List<string>();

    }
}
