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
    [Route("/api/tablerules")]
    public class TableRulesAppService : IDynamicApiController
    {
        private readonly ITableRuleService _tableRuleService;
        public TableRulesAppService(ITableRuleService tableRuleService)
        {
            _tableRuleService = tableRuleService;
        }

        [HttpGet("list")]
        public Task<List<TableRuleItemDto>> GetTableRulesAsync([FromQuery] string tableId) => _tableRuleService.GetTableRulesAsync(tableId);

        [HttpPost("save")]
        public Task CreateOrUpdateTableRulesAsync(CreateOrUpdateTableRuleDto dto) => _tableRuleService.CreateOrUpdateTableRulesAsync(dto);

        [HttpGet("init")]
        public Task InitAsync([FromQuery] string secret) => _tableRuleService.InitAsync(secret);
    }
}
