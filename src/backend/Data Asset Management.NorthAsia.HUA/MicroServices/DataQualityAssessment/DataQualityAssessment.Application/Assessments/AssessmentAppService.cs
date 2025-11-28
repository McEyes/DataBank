using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Assessments.Dtos;
using DataQualityAssessment.Application.Assessments.Services;
using DataQualityAssessment.Application.Common;
using DataQualityAssessment.Application.DataAssets.Dtos;
using DataQualityAssessment.Application.DataAssets.Services;
using DataQualityAssessment.Application.QualityRating.Dtos;

namespace DataQualityAssessment.Application.Assessments
{
    [ApiDescriptionSettings(SwaggerGroups.DataQualityAssessment)]
    [AppAuthorize]
    [Route("/api/assessment")]
    public class AssessmentAppService : IDynamicApiController
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IDataAssetService _dataAssetService;
        public AssessmentAppService(IAssessmentService assessmentService, IDataAssetService dataAssetService)
        {
            _assessmentService = assessmentService;
            _dataAssetService = dataAssetService;
        }

        [HttpGet("report")]
        public Task<List<AssessmentReportItemDto>> GetReportsAsync([FromQuery] string tableId) => _assessmentService.GetReportsAsync(tableId);

        [HttpPost("tables")]
        public Task<PagedResultDto<TablePageItemDto>> GetPagingListAsync(TableSearchDto input) => _assessmentService.GetPagingListAsync(input);

        [HttpGet("init")]
        public Task InitAsync([FromQuery] string secret) => _assessmentService.InitDataAsync(secret);

        [HttpPost("syncscore")]
        public Task SyncScoreAsync() => _dataAssetService.SyncScoreAsync();
    }
}
