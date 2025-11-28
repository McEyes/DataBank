using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataQualityAssessment.Core.Enums;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities
{
    [Table("dataquality_report")]
    public class AssessmentReportEntity : Entity<Guid>
    {
        public AssessmentReportEntity()
        {

        }
        [Column("id")]
        public override Guid Id { get => base.Id; set => base.Id = value; }
        [Column("rule_no")]
        public string RuleNo { get; set; }
        [Column("rule_name")]
        public string RuleName { get; set; }
        [Column("weight")]
        public int? Weight { get; set; }
        [Column("validate_type")]
        public RuleValidateType ValidateType { get; set; }
        [Column("table_id")]
        public string TableId { get; set; }
        [Column("score")]
        public double Score { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [Column("created_time")]
        public override DateTimeOffset CreatedTime { get => base.CreatedTime; set => base.CreatedTime = value; }
        [Column("updated_time")]
        public override DateTimeOffset? UpdatedTime { get => base.UpdatedTime; set => base.UpdatedTime = value; }

        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        [Column("batch_number")]
        public string BatchNumber { get; set; }
    }
}
