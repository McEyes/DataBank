using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Models;

namespace DataQualityAssessment.Application.RuleEngines.Rules
{
    public interface IRule
    {
        Task<bool> IsValidAsync();
        Task<double> EvaluateAsync(CancellationToken token);
        int GetWeight();
        Task<IEnumerable<AssessmentReportModel>> GetAssessmentReportsAsync();
    }
}
