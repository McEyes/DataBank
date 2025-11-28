using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Assessments.Dtos;
using DataQualityAssessment.Application.Logs.Services;
using DataQualityAssessment.Application.Rules.Services;
using DataQualityAssessment.Core.Entities;
using DataQualityAssessment.Core.Entities.DataAsset;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Application.Assessments.Services
{
    public class ExceptionLogService : IExceptionLogService, ITransient
    {
        private readonly IRepository<ExceptionLogEntity> _exceptionLogRepository;
        public ExceptionLogService(
            IRepository<ExceptionLogEntity> exceptionLogRepository)
        {
            _exceptionLogRepository = exceptionLogRepository;
        }

        public async Task WriteAsync(string typename = null, string action = null, string message = null)
        {
            if (typename == null && action == null && message == null) return;

            await _exceptionLogRepository.InsertNowAsync(new ExceptionLogEntity
            {
                Action = action,
                TypeName = typename,
                Message = message,
                CreatedTime = DateTime.Now
            });
        }
    }
}
