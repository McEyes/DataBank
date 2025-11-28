using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.ProxyApi.Dto
{
    public class DeptManagerInfo
    {
        public string id { get; set; }
        /// <summary>
        /// 部门主管名称
        /// </summary>
        public string manager_name { get; set; }
        /// <summary>
        /// 部门主管workday_id 简称wdid
        /// </summary>
        public string manager_w_d_i_d { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 上级部门
        /// </summary>
        public string parent_id { get; set; }
        /// <summary>
        /// 状态，0停用 1启用
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 类型: 1: Level1、2: Level2ll、3: Level3
        /// </summary>
        public string type { get; set; }
        public string remark { get; set; }
    }
}
