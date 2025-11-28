using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Assessments.Dtos;
using DataQualityAssessment.Core.Entities;

namespace DataQualityAssessment.Application.Logs.Services
{
    public interface IExceptionLogService
    {
        Task WriteAsync(string typename = null,string action = null,string message = null);
    }
}
