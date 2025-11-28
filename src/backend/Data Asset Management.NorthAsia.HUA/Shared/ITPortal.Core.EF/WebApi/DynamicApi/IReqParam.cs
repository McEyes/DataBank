using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;

namespace ITPortal.Core
{
    /// <summary>
    /// 返回参数信息Model
    /// </summary>
    public interface IReqParam
    {
        //    @ApiModelProperty(value = "参数名称")
        //@NotBlank(message = "参数名称不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 参数名称
        /// </summary>
        string paramName { get; set; }

        //@ApiModelProperty(value = "是否为空")
        //@NotNull(message = "是否为空不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 是否为空
        /// </summary>
        string nullable { get; set; }

        //@ApiModelProperty(value = "描述")
        //    @NotBlank(message = "描述不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 描述
        /// </summary>
        string paramComment { get; set; }

        //@ApiModelProperty(value = "操作符")
        //    @NotNull(message = "操作符不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 操作符
        /// </summary>
        string whereType { get; set; }

        //@ApiModelProperty(value = "参数类型")
        //    @NotNull(message = "参数类型不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 参数类型
        /// </summary>
        string paramType { get; set; }

        //@ApiModelProperty(value = "示例值")
        //    @NotBlank(message = "示例值不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 示例值
        /// </summary>
        string? exampleValue { get; set; }

        //@ApiModelProperty(value = "默认值")
        //    @NotBlank(message = "默认值不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 默认值
        /// </summary>
        string? defaultValue { get; set; }
    }
}
