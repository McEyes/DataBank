using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities.DataAsset
{
    [Table("tp_employee_base_info")]
    public class EmployeeBaseInfoEntity : EntityBase<string>
    {
        [Column("workday_id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        public string employee_id { get; set; }
        public string sap_id { get; set; }
        public string employee_chi_name { get; set; }
        public string ntid { get; set; }
        public string department_name { get; set; }
        public string employee_en_name { get; set; }
    }
}
