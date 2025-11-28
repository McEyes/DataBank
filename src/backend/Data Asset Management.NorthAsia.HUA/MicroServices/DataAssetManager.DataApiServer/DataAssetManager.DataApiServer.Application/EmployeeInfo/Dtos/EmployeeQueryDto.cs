using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Dtos
{



    public class EmployeeQueryDto : PageEntity<string>
    {
        public override string Id { get => WorkNTID; set => WorkNTID = value; }

        public string EmployeeCode { get; set; }
        public string EnglishName { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitle { get; set; }
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
        public string WorkEmail { get; set; }
        public string WorkNTID { get; set; }
        public string EmployeeStatus { get; set; }
        public string Workcell { get; set; }
        public string Sector { get; set; }
    }
}
