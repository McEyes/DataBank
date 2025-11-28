using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities.DataAsset
{

    [Table("metadata_source")]
    public class SourceEntity : EntityBase<string>
    {
        [Column("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        public int status { get; set; }
        public string create_by { get; set; }
        public DateTime create_time { get; set; }
        public string create_org { get; set; }
        public DateTime update_time { get; set; }
        public string remark { get; set; }
        public int db_type { get; set; }
        public string source_name { get; set; }
        public string is_sync { get; set; }
        public string db_schema { get; set; }
    }
}
