using System;
using System.ComponentModel.DataAnnotations.Schema;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities
{
    [Table("dataquality_table_rules")]
    public class TableRulesEntity : Entity<Guid>
    {
        public TableRulesEntity()
        {

        }
        [Column("id")]
        public override Guid Id { get => base.Id; set => base.Id = value; }
        [Column("rule_no")]
        public string RuleNo { get; set; }
        [Column("table_id")]
        public string TableId { get; set; }
        [Column("weight")]
        public int Weight { get; set; }
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
