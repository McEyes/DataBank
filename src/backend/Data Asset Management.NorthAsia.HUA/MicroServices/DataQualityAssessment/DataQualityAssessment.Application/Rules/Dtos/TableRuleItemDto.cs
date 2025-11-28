using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Application.Rules.Dtos
{
    public class TableRuleItemDto
    {
        public Guid Id { get; set; }
        public string RuleNo { get; set; }
        public string TableId { get; set; }
        public int Weight { get; set; }
        public DateTimeOffset CreatedTime { get; set; }

        public string Name { get; set; }
        public RuleValidateType ValidateType { get; set; }
        public RuleMonitoringLevel MonitoringLevel { get; set; }
        public string FieldType { get; set; }
        public string Source { get; set; }
        public RuleStatus Status { get; set; }
        public string Settings { get; set; }
        public string Description { get; set; }
    }
}
