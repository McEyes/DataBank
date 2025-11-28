using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Application.Rules.Dtos
{
    public class CreateOrUpdateTableRuleDto
    {
        public string TableId { get; set; }
        public List<CreateOrUpdateTableRuleItemDto> Rules { get; set; }
    }

    public class CreateOrUpdateTableRuleItemDto
    {
        public string RuleNo { get; set; }
        public int Weight { get; set; }
    }
}
