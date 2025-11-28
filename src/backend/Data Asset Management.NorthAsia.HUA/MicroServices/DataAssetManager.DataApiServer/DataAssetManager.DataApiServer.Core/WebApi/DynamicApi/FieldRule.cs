

using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace DataAssetManager.DataApiServer.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldRule
    {
        //    @ApiModelProperty(value = "字段名称")
        //@NotBlank(message = "字段名称不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 字段名称
        /// </summary>
        public string fieldName { get; set; }
        //@ApiModelProperty(value = "Mask类型")
        //@NotNull(message = "Mask类型不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// Mask类型
        /// </summary>
        public string cipherType { get; set; }
        //@ApiModelProperty(value = "规则类型")
        //    @NotNull(message = "规则类型不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 规则类型
        /// </summary>
        public string CryptType { get; set; }
    }
}
