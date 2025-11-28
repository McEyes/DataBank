using Microsoft.AspNetCore.DataProtection.KeyManagement;

using Newtonsoft.Json.Linq;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ITPortal.Core
{
    /// <summary>
    /// 
    /// </summary>
    public enum WhereType
    {
        /// <summary>
        /// 没有匹配成功,0
        /// </summary>

        [WereTypeInfo("0", "", "", "没有匹配成功")]
        None = 0,
        /// <summary>
        /// 等于,eq,=
        /// </summary>

        [WereTypeInfo("1","=","eq", "等于")]
        EQUALS = 1,
        /// <summary>
        /// 不等于,ne,!=
        /// </summary>
        [WereTypeInfo("2", "!=", "ne", "不等于")]
        NOT_EQUALS = 2,
        [WereTypeInfo("3", "LIKE", "like", "全模糊查询")]
        /// <summary>
        /// 全模糊查询,like,%key%
        /// </summary>
        LIKE = 3,
        /// <summary>
        /// 左模糊查询,likel,key%
        /// </summary>
        [WereTypeInfo("4", "LIKE", "likel", "左模糊查询")]
        LIKE_LEFT = 4,
        /// <summary>
        /// 右模糊查询,liker,%key
        /// </summary>
        [WereTypeInfo("5", "LIKE", "liker", "右模糊查询")]
        LIKE_RIGHT = 5,
        /// <summary>
        /// 大于,>,gt
        /// </summary>
        [WereTypeInfo("6", ">", "gt", "大于")]
        GREATER_THAN = 6,
        /// <summary>
        /// 大于等于,>=,ge
        /// </summary>
        [WereTypeInfo("7", ">=", "ge", "大于等于")]
        GREATER_THAN_EQUALS = 7,
        /// <summary>
        /// 小于,<,lt
        /// </summary>
        [WereTypeInfo("8", "<", "lt", "小于")]
        LESS_THAN = 8,
        /// <summary>
        /// 小于等于,<=,le
        /// </summary>
        [WereTypeInfo("9", "<=", "le", "小于等于")]
        LESS_THAN_EQUALS = 9,
        /// <summary>
        /// 是否为空,null,IS NULL
        /// </summary>
        [WereTypeInfo("10", "IS NULL", "null", "是否为空")]
        NULL = 10,
        /// <summary>
        /// 是否不为空,notnull,IS NOT NULL
        /// </summary>
        [WereTypeInfo("11", "IS NOT NULL", "notnull", "是否不为空")]
        NOT_NULL = 11,
        /// <summary>
        /// IN, in (...)
        /// </summary>
        [WereTypeInfo("12", "IN", "in", "IN")]
        IN = 12,
        /// <summary>
        /// 在...之间,between, between .. and ..  
        /// </summary>
        [WereTypeInfo("13", "BETWEEN", "between", "BETWEEN")]
        BETWEEN = 13
    }

}
