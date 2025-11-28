using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.SqlParser.Models
{
    public class OrderInfo
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 排序方向 (ASC/DESC)
        /// </summary>
        public OrderByType OrderByType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="field">排序字段</param>
        /// <param name="sort">排序方向</param>
        public OrderInfo(string field, string sort)
        {
            Field = field;
            sort = sort?.ToUpper() ?? "ASC";
            if (sort == "ASC")
                OrderByType = OrderByType.Asc;
            else
                OrderByType = OrderByType.Desc;
        }
    }
}