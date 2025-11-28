using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataQualityAssessment.Core.Enums;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities
{
    [Table("dataquality_result")]
    public class AssessmentResultEntity : Entity<string>
    {
        public AssessmentResultEntity()
        {

        }

        [Column("table_id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        [Column("execution_time")]
        public DateTimeOffset ExecutionTime { get; set; }
        [Column("end_time")]
        public DateTimeOffset? EndTime { get; set; }
        [Column("elapsedmilliseconds")]
        public double? ElapsedMilliseconds { get; set; }
        [Column("status")]
        public ProgressStatus Status { get; set; }
        [Column("repeat_count")]
        public int? RepeatCount { get; set; }
        [Column("last_exception")]
        public string LastException { get; set; }
        [Column("error_type")]
        public TableErrorType? ErrorType { get; set; }
        [Column("score")]
        public double? Score { get; set; }
        [Column("last_score")]
        public double? LastScore { get; set; }
        [Column("created_time")]
        public override DateTimeOffset CreatedTime { get => base.CreatedTime; set => base.CreatedTime = value; }
        [Column("updated_time")]
        public override DateTimeOffset? UpdatedTime { get => base.UpdatedTime; set => base.UpdatedTime = value; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
    }
}
