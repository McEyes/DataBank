using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Application.Assessments.Dtos
{
    public class AssessmentResultDto
    {
        /// <summary>
        /// table id
        /// </summary>
        public string Id { get; set; }
        public DateTimeOffset ExecutionTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public double? ElapsedMilliseconds { get; set; }
        public ProgressStatus Status { get; set; }
        public double? Score { get; set; }
    }
}
