using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Application.Assessments.Dtos
{
    public class AssessmentReportItemDto
    {
        public string RuleNo { get; set; }
        public string RuleName { get; set; }
        public int? Weight { get; set; }
        public RuleValidateType ValidateType { get; set; }
        public RuleMonitoringLevel MonitoringLevel { get; set; }
        public string TableId { get; set; }
        public double Score { get; set; }
        public string Description { get; set; }
        public string Fields { get; set; }
    }
}
