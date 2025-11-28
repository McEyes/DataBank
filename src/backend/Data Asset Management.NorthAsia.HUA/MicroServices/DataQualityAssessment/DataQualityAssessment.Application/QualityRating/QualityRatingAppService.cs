using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Common;
using DataQualityAssessment.Application.QualityRating.Dtos;
using DataQualityAssessment.Application.QualityRating.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DataQualityAssessment.Application.QualityRating
{
    [ApiDescriptionSettings(SwaggerGroups.DataQualityAssessment)]
    [AppAuthorize]
    [Route("/api/qualityrating")]
    public class QualityRatingAppService : IDynamicApiController
    {
        private readonly IQualityRatingService _qualityRatingService;
        
        public QualityRatingAppService(IQualityRatingService qualityRatingService)
        {
            _qualityRatingService = qualityRatingService;
        }

        [AllowAnonymous]
        [HttpPost("addtoevaluate")]
        public Task AddToEvaluateAsync(AddToEvaluateDto dto) => _qualityRatingService.AddToEvaluateAsync(dto);
    }
}
