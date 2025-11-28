using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ITPortal.Core
{
    /// <summary>
    /// 执行配置信息Model
    /// </summary>
    public class ExecuteConfig
    {

        //    @ApiModelProperty(value = "数据源")
        //@NotBlank(message = "数据源不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 数据源
        /// </summary>
        public string sourceId{ get; set; }

        //@ApiModelProperty(value = "配置方式")
        //@NotNull(message = "配置方式不能为空", groups = { ValidationGroups.Insert.class, ValidationGroups.Update.class})
        /// <summary>
        /// 配置方式
        /// </summary>
        public string configType { get; set; }

        //@ApiModelProperty(value = "数据库表主键")
        /// <summary>
        /// 数据库表主键
        /// </summary>
        public string tableId { get; set; }

        //@ApiModelProperty(value = "数据库表")
        /// <summary>
        /// 数据库表主键
        /// </summary>
        public string tableName { get; set; }

        //@ApiModelProperty(value = "表字段列表")
        /// <summary>
        /// 表字段列表
        /// </summary>
        //    @Valid
        public List<FieldParam> fieldParams { get; set; }

        //@ApiModelProperty(value = "解析SQL")
        /// <summary>
        /// 解析SQL
        /// </summary>
        public string sqlText { get; set; }

        // 创建 API 的时候，先检查 Meta Table 里面的 JsonSqlConfig 是否存在 Limit，否则使用默认值
        //@ApiModelProperty(value = "分页数最大阈值")
        /// <summary>
        /// 分页数最大阈值
        /// </summary>
        public int? pageSizeLimit { get; set; }
    }
}
