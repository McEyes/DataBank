using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Models
{
    public class MetadataTableModel
    {
        public string id { get; set; }
        public string source_id { get; set; }
        public string table_name { get; set; }
        public string table_comment { get; set; }

        public string update_frequency { get; set; }
        public string update_method { get; set; }
        public string data_category { get; set; }
    }
}
