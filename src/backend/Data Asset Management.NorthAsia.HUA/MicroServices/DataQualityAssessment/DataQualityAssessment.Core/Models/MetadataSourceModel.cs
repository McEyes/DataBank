using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Models
{
    public class MetadataSourceModel
    {
        public string id { get; set; }
        public int status { get; set; }
        public string source_name { get; set; }
        public int db_type { get; set; }
        public string db_schema { get; set; }
        public string remark { get; set; }
    }
}
