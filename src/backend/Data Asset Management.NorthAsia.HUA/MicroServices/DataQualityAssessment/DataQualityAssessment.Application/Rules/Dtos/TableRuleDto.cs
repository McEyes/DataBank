using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Application.Rules.Dtos
{
    public class TableRuleDto
    {
        public string RuleNo { get; set; }
        public int Weight { get; set; }
        public RuleValidateType ValidateType { get; set; }
    }
}
