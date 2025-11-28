using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Application.Assessments.Dtos
{
    public class TablePageItemDto
    {
        public string TableId { get; set; }
        public string TableName { get; set; }
        public string DbName { get; set; }
        public string SourceId { get; set; }
        public string SourceName { get; set; }
        public string TableComment { get; set; }
        public string RecordCount { get; set; }
        public string UpdateFrequency { get; set; }
        public int? Score { get; set; }
        public int? LastScore { get; set; }
        public int RuleCount { get; set; }
        public string DataOwner { get; set; }
        public string OwnerDept { get; set; }
        public DateTimeOffset? CreatedTableTime { get; set; }
        public DateTimeOffset? ExecutionTime { get; set; }
        public ProgressStatus? Status { get; set; }
    }
}
