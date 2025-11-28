using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.EmployeeInfos.Dtos
{


    [SugarTable(TableName = "tp_employee_base_info")]
    public class EmployeeBaseInfo : Entity<string>
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "workday_id")]
        public override string Id { get; set; }

        public string employee_id { get; set; }
        [SugarColumn(ColumnName = "ntid")]
        public string Ntid { get; set; }
        [SugarColumn(ColumnName = "work_email")]
        public string Work_email { get; set; }

        [SugarColumn(ColumnName = "employee_chi_name")]
        public string Employee_chi_name { get; set; }
        [SugarColumn(ColumnName = "employee_first_name")]
        public string Employee_first_name { get; set; }
        [SugarColumn(ColumnName = "employee_last_name")]
        public string Employee_last_name { get; set; }
        [SugarColumn(ColumnName = "sap_id")]
        public string Sap_id { get; set; }

        [SugarColumn(ColumnName = "department_name")]
        public string Department_name { get; set; }
        public string job_classification { get; set; }
        public string employee_worktype { get; set; }
        public string work_space { get; set; }
        public string onboard_date { get; set; }
        public string job_category { get; set; }
        public string supplier { get; set; }
        public string profit_center { get; set; }
        public string probation_end_date { get; set; }
        public string hua_id { get; set; }
        public string supervisor_org_id { get; set; }
        public string hire_date { get; set; }
        public string business_title { get; set; }
        public string company_code { get; set; }
        public string company_location { get; set; }
        public string cost_center_id { get; set; }
        public string direct_manager { get; set; }
        public string direct_manager_email { get; set; }
        public string direct_manager_employee_id { get; set; }
        public string direct_manager_ntid { get; set; }
        public string direct_manager_wdid { get; set; }
        public string employee_nationality { get; set; }
        public string employee_workcell { get; set; }
        public string global_job_title { get; set; }
        public string higher_direct_manager { get; set; }
        public string higher_direct_manager_email { get; set; }
        public string higher_direct_manager_employee_id { get; set; }
        public string higher_direct_manager_ntid { get; set; }
        public string higher_direct_manager_wdid { get; set; }
        public string job_family_group { get; set; }
        //public string workday_id => Id;
    }
}
