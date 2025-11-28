using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.QualityRating.Dtos;
using DataQualityAssessment.Core.Models;

namespace DataQualityAssessment.Application.QualityRating.Services
{
    public interface IQualityRatingService
    {
        Task EvaluateAsync(CancellationToken token);
        Task AddToEvaluateAsync(AddToEvaluateDto dto);
        void CloseAllConnections();
    }
}
