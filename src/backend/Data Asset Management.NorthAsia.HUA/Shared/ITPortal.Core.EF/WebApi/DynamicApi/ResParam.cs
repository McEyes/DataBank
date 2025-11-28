

namespace ITPortal.Core
{
    /// <summary>
    /// 请求参数信息Model
    /// </summary>
    public class ResParam
    {


        //    @ApiModelProperty(value = "字段名称")
        //@NotBlank(message = "字段名称不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 字段名称
        /// </summary>
        public string fieldName { get; set; }

        //@ApiModelProperty(value = "描述")
        //@NotBlank(message = "描述不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 描述
        /// </summary>
        public string fieldComment { get; set; }

        //@ApiModelProperty(value = "数据类型")
        //    @NotNull(message = "数据类型不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 数据类型
        /// </summary>
        public string dataType { get; set; }

        //@ApiModelProperty(value = "示例值")
        //    @NotBlank(message = "示例值不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 示例值
        /// </summary>
        public string exampleValue { get; set; }

        //@ApiModelProperty(value = "字段别名")
        /// <summary>
        /// 字段别名
        /// </summary>
        public string? fieldAliasName { get; set; }
    }
}
