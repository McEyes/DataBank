using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class AssetClientDto : PageEntity<Guid>
    {

        /// <summary>
        /// 申请类型：1 Individual个人,2 Application应用程序
        /// </summary>
        public int? ClientType { get; set; }

        /// <summary>
        /// client code,个人是保存的是申请人id(=owner)，应用程序是新id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// client code
        /// </summary>
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
        /// client code
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool? Enabled { get; set; }
        /// <summary>
        /// 拥有人
        /// </summary>
        public string? Owner { get; set; }
        /// <summary>
        /// 拥有人姓名
        /// </summary>
        public string? OwnerName { get; set; }

        /// <summary>
        /// 拥有人部门
        /// </summary>
        public string? OwnerDept { get; set; }

        /// <summary>
        /// 拥有人部门
        /// </summary>
        public string? OwnerNtid { get; set; }
        /// <summary>
        /// 访问人IP白名单
        /// </summary>
        public string? WhiteipList { get; set; }
        /// <summary>
        /// sme_list
        /// </summary>
        [SugarColumn(ColumnName = "sme_list", IsJson = true)]
        public List<StaffUser>? SMEList { get; set; }
        /// <summary>
        /// 类别（Global/Regional/Site）
        /// </summary>
        [SugarColumn(ColumnName = "belong_area")]
        public string? BelongArea { get; set; }
        /// <summary>
        /// 主数据类型（多选）Json array
        /// </summary>
        [SugarColumn(ColumnName = "master_data_types", IsJson = true)]
        public List<string>? MasterDataTypes { get; set; }
        public List<AssetClientSecretsEntity>? Secrets { get; set; }=new List<AssetClientSecretsEntity>();
        public List<AssetClientScopesEntity>? Scopes { get; set; }=new List<AssetClientScopesEntity>();

    }

    public class StaffUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
