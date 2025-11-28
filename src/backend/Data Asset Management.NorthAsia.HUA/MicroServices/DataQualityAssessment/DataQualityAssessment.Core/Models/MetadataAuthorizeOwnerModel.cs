using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Models
{
    public class MetadataAuthorizeOwnerModel
    {
        public string object_id { get; set; }
        public string object_type { get; set; }
        public string owner_id { get; set; }
        public string owner_dept { get; set; }
        public string owner_name { get; set; }
    }
}
