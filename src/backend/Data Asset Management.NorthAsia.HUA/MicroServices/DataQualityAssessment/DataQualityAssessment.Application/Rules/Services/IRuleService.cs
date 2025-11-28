using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Rules.Dtos;
using DataQualityAssessment.Core.Entities;
using DataQualityAssessment.Core.Models;

namespace DataQualityAssessment.Application.Rules.Services
{
    public interface IRuleService
    {
        Task<List<RuleDto>> GetAllRulesAsync(string ruleName);
    }
}
