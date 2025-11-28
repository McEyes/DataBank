using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Core.Models
{
    public class AssessmentReportModel
    {
        public string RuleNo { get; set; }
        public string RuleName { get; set; }
        public int? Weight { get; set; }
        public RuleValidateType ValidateType { get; set; }
        public string TableId { get; set; }
        public double Score { get; set; }
        public string Description { get; set; }
        public string BatchNumber { get; set; }
    }
}
