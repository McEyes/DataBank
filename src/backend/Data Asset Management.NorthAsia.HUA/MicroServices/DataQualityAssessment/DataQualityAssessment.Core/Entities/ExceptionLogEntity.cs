using System;
using System.ComponentModel.DataAnnotations.Schema;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities
{
    [Table("dataquality_exception_log")]
    public class ExceptionLogEntity : Entity<Guid>
    {
        public ExceptionLogEntity()
        {

        }
        
        [Column("id")]
        public override Guid Id { get => base.Id; set => base.Id = value; }
        [Column("type_name")]
        public string TypeName { get; set; }
        [Column("action")]
        public string Action { get; set; }
        [Column("message")]
        public string Message { get; set; }
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
