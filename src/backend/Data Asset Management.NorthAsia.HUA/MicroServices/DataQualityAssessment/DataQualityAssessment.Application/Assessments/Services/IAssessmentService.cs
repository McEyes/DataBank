using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Assessments.Dtos;
using DataQualityAssessment.Application.Common;
using DataQualityAssessment.Application.DataAssets.Dtos;
using DataQualityAssessment.Core.Entities;

namespace DataQualityAssessment.Application.Assessments.Services
{
    public interface IAssessmentService
    {
        Task SaveReportsAsync(IEnumerable<AssessmentReportEntity> entities);
        Task<List<AssessmentReportItemDto>> GetReportsAsync(string tableId);
        Task AddToEvaluateAsync(string tableId);
        Task BeginEvaluateAsync(string tableId);
        Task EndEvaluateAsync(string tableId, double score);
        Task SaveExceptionAsync(string tableId, string message);
        Task<IEnumerable<AssessmentResultDto>> GetWaitingListAsync();
        Task<PagedResultDto<TablePageItemDto>> GetPagingListAsync(TableSearchDto input);
        Task InitDataAsync(string secret);
    }
}
