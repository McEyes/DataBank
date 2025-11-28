using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;

namespace ITPortal.Core
{
    /// <summary>
    /// 返回参数信息Model
    /// </summary>
    public class ReqParam: IReqParam
    {
        //    @ApiModelProperty(value = "参数名称")
        //@NotBlank(message = "参数名称不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 参数名称
        /// </summary>
        public string paramName { get; set; }

        //@ApiModelProperty(value = "是否为空")
        //@NotNull(message = "是否为空不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 是否为空
        /// </summary>
        public string nullable { get; set; }

        //@ApiModelProperty(value = "描述")
        //    @NotBlank(message = "描述不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 描述
        /// </summary>
        public string paramComment { get; set; }

        //@ApiModelProperty(value = "操作符")
        //    @NotNull(message = "操作符不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 操作符
        /// </summary>
        public string whereType { get; set; }

        //@ApiModelProperty(value = "参数类型")
        //    @NotNull(message = "参数类型不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 参数类型
        /// </summary>
        public string paramType { get; set; }

        //@ApiModelProperty(value = "示例值")
        //    @NotBlank(message = "示例值不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})

        /// <summary>
        /// 示例值
        /// </summary>
        public string? exampleValue { get; set; }

        //@ApiModelProperty(value = "默认值")
        //    @NotBlank(message = "默认值不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 默认值
        /// </summary>
        public string? defaultValue { get; set; }
    }
}
