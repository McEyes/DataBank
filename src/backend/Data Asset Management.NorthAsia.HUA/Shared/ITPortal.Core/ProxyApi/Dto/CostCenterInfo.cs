using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.ProxyApi.Dto
{
    public class CostCenterInfo
    {
        public string id { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string area { get; set; }
        /// <summary>
        /// 职业发展规划-部门
        /// </summary>
        public string career_path_department { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 成本中心所有者
        /// </summary>
        public string cc_owner_eng_name { get; set; }
        /// <summary>
        /// 	成本中心所有者WD工号
        /// </summary>
        public string cc_owner_wd_id { get; set; }
        /// <summary>
        /// 		主键，成本中心
        /// </summary>
        public string cost_center_id { get; set; }
        /// <summary>
        /// 所属组织
        /// </summary>
        public string create_org { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string department_name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 	工厂级别部门经理
        /// </summary>
        public string plant_fm_eng_name { get; set; }
        /// <summary>
        /// 		工厂级别部门经理WD工号
        /// </summary>
        public string plant_fm_wd_id { get; set; }
        /// <summary>
        /// 		工厂端人力资源部负责人
        /// </summary>
        public string plant_hr_head_eng_name { get; set; }
        /// <summary>
        /// 		工厂端人力资源部负责人WD工号
        /// </summary>
        public string plant_hr_head_wd_id { get; set; }
        /// <summary>
        /// 		工厂端3 Packs
        /// </summary>
        public string plant_three_packs_eng_name { get; set; }
        /// <summary>
        /// 		工厂端3 Packs WD工号
        /// </summary>
        public string plant_three_packs_wd_id { get; set; }
        /// <summary>
        /// 		备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 			部门Sector经理
        /// </summary>
        public string sector_fm_eng_name { get; set; }
        /// <summary>
        /// 			部门Sector经理WD工号
        /// </summary>
        public string sector_fm_wd_id { get; set; }
        /// <summary>
        /// 			Sector运营负责人
        /// </summary>
        public string sector_head_eng_name { get; set; }
        /// <summary>
        /// 				Sector运营负责人WD工号
        /// </summary>
        public string sector_head_wd_id { get; set; }
        /// <summary>
        /// 				sector_name
        /// </summary>
        public string Sector { get; set; }
        /// <summary>
        /// 		状态，0停用 1启用
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 		类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 		版本号
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 		工作坊经理
        /// </summary>
        public string wc_manager_eng_name { get; set; }
        /// <summary>
        /// 		工作坊经理WD工号
        /// </summary>
        public string wc_manager_wd_id { get; set; }
        /// <summary>
        /// 		工作坊
        /// </summary>
        public string workcell_name { get; set; }
    }
}
