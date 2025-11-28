
using DataQualityAssessment.Application.DataAssets.Services;
using DataQualityAssessment.Application.QualityRating.Dtos;
using DataQualityAssessment.Application.QualityRating.Services;

namespace DataQualityAssessment.Application.Common
{
    [ApiDescriptionSettings(SwaggerGroups.DataQualityAssessment)]
    [AppAuthorize]
    [Route("/api/common")]
    public class CommonAppService : IDynamicApiController
    {
        private readonly ICommonService _commonService;

        public CommonAppService(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet("departments")]
        public List<string> GetAllDepartments() => _commonService.GetAllDepartments();
    }
}
