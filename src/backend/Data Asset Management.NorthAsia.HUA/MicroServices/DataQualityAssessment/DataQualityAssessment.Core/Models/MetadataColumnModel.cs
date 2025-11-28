using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Models
{
    public class MetadataColumnModel
    {
        public string source_id { get; set; }
        public string table_id { get; set; }
        public string column_name { get; set; }
        public string column_comment { get; set; }
        public string column_key { get; set; }
        public string column_nullable { get; set; }
        public string column_position { get; set; }
        public string data_type { get; set; }
        public string data_length { get; set; }
    }
}
