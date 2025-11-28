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
    public interface ITableRuleService
    {
        Task<List<TableRuleDto>> GetRuleListAsync(string tableId);
        Task CreateOrUpdateTableRulesAsync(CreateOrUpdateTableRuleDto input);
        Task<List<TableRuleItemDto>> GetTableRulesAsync(string tableId);
        Task InitAsync(string secret);

    }
}
