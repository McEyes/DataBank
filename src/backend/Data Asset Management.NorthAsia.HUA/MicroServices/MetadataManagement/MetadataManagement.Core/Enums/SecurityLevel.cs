
using System.ComponentModel;

namespace DataAssetManager.DataApiServer.Application
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    public enum SecurityLevel
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        //[Display(Name ="")]
        None =0,
        /// <summary>
        /// 公众级
        /// </summary>
        [Description("公众级")]
        //[Display(Name = "公众级")]
        Public = 1,
        /// <summary>
        /// 内部使用级
        /// </summary>
        [Description("内部使用级")]
        //[Display(Name = "内部使用级")]
        InternalUse = 2,
        /// <summary>
        /// 机密
        /// </summary>
        [Description("机密")]
        //[Display(Name = "机密")]
        Confidential = 3,
        /// <summary>
        /// 受限机密
        /// </summary>
        [Description("受限机密")]
        //[Display(Name = "受限机密")]
        RestrictedConfidential = 4,
        /// <summary>
        /// 注册机密
        /// </summary>
        [Description("注册机密")]
        //[Display(Name = "注册机密")]
        RegisteredConfidential = 5,
    }
}
