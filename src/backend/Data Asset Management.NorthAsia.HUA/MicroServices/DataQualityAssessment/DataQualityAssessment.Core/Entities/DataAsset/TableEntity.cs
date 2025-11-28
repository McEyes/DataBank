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
    [Table("metadata_table")]
    public class TableEntity : EntityBase<string>
    {
        [Column("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        public string source_id { get; set; }
        public string table_name { get; set; }
        public string table_comment { get; set; }
        public string update_frequency { get; set; }
        public string update_method { get; set; }
        public string data_category { get; set; }
        //public string column11 { get; set; } // record count
        //public string column7 { get; set; }
        public int? status { get; set; }
        public int? quality_score { get; set; }
        public int? last_score { get; set; }
        public DateTimeOffset? create_time { get; set; }
    }
}
