using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{

    /// <summary>
    /// 人员信息表
    /// </summary>
    [Serializable]
    [SugarTable("tp_employee_base_info")]
    public class EmployeeBaseInfoEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string? ntid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string workday_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? employee_chi_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? employee_last_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? employee_first_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? work_email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? employee_workcell { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? employee_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? department_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? direct_manager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? direct_manager_wdid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? direct_manager_email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? direct_manager_ntid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? direct_manager_employee_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? company_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? company_location { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? job_family_group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? business_title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? cost_center_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? global_job_title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? employee_nationality { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? higher_direct_manager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? higher_direct_manager_email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? higher_direct_manager_wdid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? higher_direct_manager_ntid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? higher_direct_manager_employee_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? job_classification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? employee_worktype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? work_space { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? onboard_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? job_category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? supplier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? profit_center { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? probation_end_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? sap_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? hua_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? supervisor_org_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? hire_date { get; set; }

    }
}
