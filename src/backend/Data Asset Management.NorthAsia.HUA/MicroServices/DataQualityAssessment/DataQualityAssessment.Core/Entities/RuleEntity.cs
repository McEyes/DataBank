using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataQualityAssessment.Core.Enums;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities
{
    [Table("dataquality_rule")]
    public class RuleEntity : Entity<string>
    {
        public RuleEntity()
        {

        }

        [Column("rule_no")]
        public override string Id { get => base.Id; set => base.Id = value; }
        [Column("name")]
        public string Name { get; set; }
        [Column("validate_type")]
        public RuleValidateType ValidateType { get; set; }
        [Column("monitoring_level")]
        public RuleMonitoringLevel MonitoringLevel { get; set; }
        [Column("field_type")]
        public string FieldType { get; set; }
        [Column("source")]
        public string Source { get; set; }
        [Column("status")]
        public RuleStatus Status { get; set; }
        [Column("settings")]
        public string Settings { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("sampling_rules_description")]
        public string SamplingRulesDescription { get; set; }

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
