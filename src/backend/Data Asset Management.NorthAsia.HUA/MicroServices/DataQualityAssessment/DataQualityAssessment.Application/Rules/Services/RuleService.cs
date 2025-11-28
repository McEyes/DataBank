using DataQualityAssessment.Application.Rules.Dtos;
using DataQualityAssessment.Core.Entities;
using DataQualityAssessment.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DataQualityAssessment.Application.Rules.Services
{
    public class RuleService : IRuleService, ITransient
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public RuleService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<List<RuleDto>> GetAllRulesAsync(string ruleName)
        {
            var userInfo = App.HttpContext.GetCurrUserInfo();
           
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var ruleRepository = Db.GetRepository<RuleEntity>(serviceProvider);
                var list = await ruleRepository.Where(!string.IsNullOrWhiteSpace(ruleName), t => t.Name.Contains(ruleName)).ToListAsync();
                if (list == null) Oops.Oh($"No data");
                return list.Adapt<List<RuleDto>>();
            }
        }
    }
}
