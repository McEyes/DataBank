using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Application.Account.Dtos
{


    [SugarTable(TableName = "tp_employee_base_info")]
    public class EmployeeInfo : Entity<string>
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "workday_id")]
        public override string Id { get; set; }
        [SugarColumn(ColumnName = "ntid")]
        public string WorkNTID { get; set; }
        [SugarColumn(ColumnName = "work_email ")]
        public string WorkEmail { get; set; }
        [SugarColumn(ColumnName = "employee_workcell ")]
        public string Workcell { get; set; }

        [SugarColumn(ColumnName = "employee_id ")]
        public string EmployeeCode { get; set; }
        [SugarColumn(ColumnName = "department_name ")]
        public string DepartmentName { get; set; }

        [SugarColumn(ColumnName = "employee_last_name ")]
        public string EnglishLastName { get; set; }

        [SugarColumn(ColumnName = "employee_first_name ")]
        public string EnglishFirstName { get; set; }

        public string EnglishName => $"{EnglishFirstName} {EnglishLastName}";
        [SugarColumn(ColumnName = "employee_chi_name ")]
        public string ChineseName { get; set; }

        public string Name { get { return WorkEmail?.Replace("@jabil.com", "", StringComparison.CurrentCultureIgnoreCase).Replace("_", " "); } }

        [SugarColumn(ColumnName = "global_job_title ")]
        public string JobTitle { get; set; }

        [SugarColumn(ColumnName = "business_title ")]
        public string BusinessTitleLocal { get; set; }

        public string JobFamily { get; set; }
        public string JobFamilyGroup { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string PlantDivision { get; set; }
        public string ManagementDivision { get; set; }
        public string LegalEntity { get; set; }
        public string CompanyCode { get; set; }
        public string ManagerNTID { get; set; }
        public string ManagerEmail { get; set; }
        public string EmployeeStatus { get; set; }
        public string Sector { get; set; }
    }
}
