using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Common;
using DataQualityAssessment.Application.Rules.Dtos;
using DataQualityAssessment.Application.Rules.Services;

namespace DataQualityAssessment.Application.Rules
{
    [ApiDescriptionSettings(SwaggerGroups.DataQualityAssessment)]
    [AppAuthorize]
    [Route("/api/rules")]
    public class RuleAppService : IDynamicApiController
    {
        private readonly IRuleService _ruleService;
        public RuleAppService(IRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        [HttpGet("all")]
        public Task<List<RuleDto>> GetAllRulesAsync([FromQuery] string ruleName) => _ruleService.GetAllRulesAsync(ruleName);
    }
}
