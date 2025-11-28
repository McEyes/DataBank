using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Assessments.Services;
using DataQualityAssessment.Application.RuleEngines.Rules;
using DataQualityAssessment.Core.Entities;
using DataQualityAssessment.Core.Models;

namespace DataQualityAssessment.Application.RuleEngines
{
    public class RuleEngine
    {
        private readonly List<IRule> _rules = new();
        public void AddRule(IRule rule)
        {
            _rules.Add(rule);
        }

        private async Task<int> CheckWeightAsync(CancellationToken token)
        {
            int totalWeight = 0;
            foreach (var rule in _rules)
            {
                await rule.IsValidAsync();

                var weight = rule.GetWeight();
                totalWeight += weight;

                if (token.IsCancellationRequested)
                    break;
            }

            if (totalWeight != 100)
            {
                throw new Exception("The sum of all rule weights must equal 100");
            }

            return totalWeight;
        }

        public async Task<double> EvaluateAsync(CancellationToken token)
        {
            double validSumScore = 0;
            List<AssessmentReportModel> reportModels = new List<AssessmentReportModel>();

            //Check rule settings
            var totalWeight = await this.CheckWeightAsync(token);

            foreach (var rule in _rules)
            {
                var validScore = await rule.EvaluateAsync(token);
                var weight = rule.GetWeight();
                validSumScore += validScore * weight;
                reportModels.AddRange(await rule.GetAssessmentReportsAsync());

                if (token.IsCancellationRequested)
                    break;
            }

            if (reportModels != null && reportModels.Count > 0)
            {
                var batchNumber = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                reportModels.ForEach(t => t.BatchNumber = batchNumber);
                var assessmentReportService = App.GetService<IAssessmentService>();
                await assessmentReportService.SaveReportsAsync(reportModels.Adapt<List<AssessmentReportEntity>>());
            }

            return validSumScore / totalWeight;
        }
    }
}
