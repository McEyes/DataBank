using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.DataSource
{
    public enum ConfigType
    {
        /// <summary>
        /// 表引导模式
        /// </summary>
        [Description("表引导模式")]
        FORM = 1,//("1", "表引导模式"),
        /// <summary>
        /// 脚本模式
        /// </summary>
        [Description("脚本模式")]
        SCRIPT = 2,//("2", "脚本模式"),
        /// <summary>
        /// SQL模式
        /// </summary>
        [Description("SQL模式")]
        SQL = 3,//("3", "SQL模式"),
        /// <summary>
        /// json模式
        /// </summary>
        [Description("json模式")]
        JSON = 4//("4", "json模式");
    }
}
