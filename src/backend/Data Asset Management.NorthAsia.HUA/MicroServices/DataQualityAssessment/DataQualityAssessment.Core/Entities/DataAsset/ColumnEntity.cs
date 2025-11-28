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
    [Table("metadata_column")]
    public class ColumnEntity : EntityBase<string>
    {
        [Column("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        public string source_id { get; set; }
        public string table_id { get; set; }
        public string column_name { get; set; }
        public string column_comment { get; set; }
        public string column_key { get; set; }
        public string column_nullable { get; set; }
        public int? column_position { get; set; }
        public string data_type { get; set; }
        public string data_length { get; set; }
    }
}
