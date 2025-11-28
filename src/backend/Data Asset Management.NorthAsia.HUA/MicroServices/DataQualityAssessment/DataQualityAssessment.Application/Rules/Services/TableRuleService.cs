using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.RuleEngines;
using DataQualityAssessment.Application.RuleEngines.Rules;
using DataQualityAssessment.Application.Rules.Dtos;
using DataQualityAssessment.Application.Rules.Services;
using DataQualityAssessment.Core.DbContextLocators;
using DataQualityAssessment.Core.Entities;
using DataQualityAssessment.Core.Entities.DataAsset;
using DataQualityAssessment.Core.Models;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DataQualityAssessment.Application.Rules.Services
{
    public class TableRuleService : ITableRuleService, ITransient
    {
        private IServiceScopeFactory _scopeFactory;
        public TableRuleService(
            IServiceScopeFactory scopeFactory
            )
        {
            _scopeFactory = scopeFactory;
        }

        public async Task CreateOrUpdateTableRulesAsync(CreateOrUpdateTableRuleDto input)
        {
            ArgumentNullException.ThrowIfNull(input, "input");
            ArgumentNullException.ThrowIfNull(input.Rules, "input.Rules");

            var validRules = input.Rules.Where(t => t.Weight > 0 && t.Weight <= 100)?.ToList();
            if (validRules == null || validRules.Count == 0)
            {
                Oops.Oh($"Some rules have invalid weight settings.");
            }
            var sum = validRules.Sum(t => t.Weight);
            ArgumentOutOfRangeException.ThrowIfNotEqual(sum, 100, "The sum of all rule weights must equal 100");

            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var tableRuleRepository = Db.GetRepository<TableRulesEntity>(serviceProvider);
                var ruleRepository = Db.GetRepository<RuleEntity>(serviceProvider);

                if (validRules.GroupBy(t => t.RuleNo).Count() != validRules.Count)
                {
                    Oops.Oh($"There are duplicate rules");
                }

                var rules = ruleRepository.Where(t => validRules.Select(a => a.RuleNo).Any(a => a == t.Id)).ToList();
                if (rules.Count != validRules.Count)
                {
                    Oops.Oh($"Some rules don't exist");
                }

                var tableRules = tableRuleRepository.Where(t => t.TableId == input.TableId).ToList();
                await tableRuleRepository.DeleteAsync(tableRules);

                foreach (var item in validRules)
                {
                    var entity = new TableRulesEntity
                    {
                        CreatedTime = DateTime.Now,
                        RuleNo = item.RuleNo,
                        TableId = input.TableId,
                        Weight = item.Weight
                    };

                    await tableRuleRepository.InsertAsync(entity);
                }

                await tableRuleRepository.SaveNowAsync();
            }
        }

        public async Task InitAsync(string secret)
        {
            if (secret != "123") Oops.Oh($"secret error!");

            using (var scope = _scopeFactory.CreateScope())
            {
                var serivce = scope.ServiceProvider;
                var tableRepository = Db.GetRepository<TableEntity, DataAssetDbContextLocator>(serivce);
                var tableRuleRepository = Db.GetRepository<TableRulesEntity>(serivce);

                var query = tableRepository.AsQueryable();
                var tables = query.ToList();
                var dict = new Dictionary<string, int> {
                        { "CN02QAIT20250001",10},
                        { "CN02QAIT20250002",10},
                        { "CN02QAIT20250003",10},
                        { "CN02QAIT20250005",10},
                        { "CN02QAIT20250006",10},
                        { "CN02QAIT20250007",15},
                        { "CN02QAIT20250008",15},
                        { "CN02QAIT20250009",20}
                    };

                foreach (var item in tables)
                {
                    var tableRules = tableRuleRepository.Where(t => t.TableId == item.Id);
                    if (tableRules != null)
                    {
                        await tableRuleRepository.DeleteAsync(tableRules);
                    }

                    foreach (var rule in dict)
                    {
                        await tableRuleRepository.InsertAsync(new TableRulesEntity { RuleNo = rule.Key, TableId = item.Id, Weight = rule.Value });
                    }

                    await tableRuleRepository.SaveNowAsync();
                }
            }
        }

        public async Task<List<TableRuleDto>> GetRuleListAsync(string tableId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var ruleRepository = Db.GetRepository<RuleEntity>(serviceProvider);
                var tableRuleRepository = Db.GetRepository<TableRulesEntity>(serviceProvider);
                var ruleQuery = ruleRepository.AsQueryable();
                var tableRuleQuery = tableRuleRepository.AsQueryable();
                var result = await tableRuleQuery.Where(t => t.TableId == tableId).Join(ruleQuery, a => a.RuleNo, a => a.Id, (a, b) => new TableRuleDto
                {
                    RuleNo = a.RuleNo,
                    ValidateType = b.ValidateType,
                    Weight = a.Weight
                }).ToListAsync();
                return result;
            }
        }

        public async Task<List<TableRuleItemDto>> GetTableRulesAsync(string tableId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var ruleRepository = Db.GetRepository<RuleEntity>(serviceProvider);
                var tableRuleRepository = Db.GetRepository<TableRulesEntity>(serviceProvider);

                var queryTableRule = tableRuleRepository.AsQueryable();
                var queryRule = ruleRepository.Where(t=>t.Status == Core.Enums.RuleStatus.Publish);
                var results = queryTableRule.Where(t => t.TableId == tableId).Join(queryRule, a => a.RuleNo, a => a.Id, (a, b) => new TableRuleItemDto
                {
                    Description = b.Description,
                    FieldType = b.FieldType,
                    MonitoringLevel = b.MonitoringLevel,
                    Name = b.Name,
                    RuleNo = b.Id,
                    Settings = b.Settings,
                    Source = b.Source,
                    Status = b.Status,
                    ValidateType = b.ValidateType,

                    TableId = a.TableId,
                    CreatedTime = a.CreatedTime,
                    Weight = a.Weight
                }).ToList();

                if (results == null) Oops.Oh($"No data");
                return await Task.FromResult(results);
            }
        }
    }
}
