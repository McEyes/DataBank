using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Openlineage.Models
{
    public class DatasetFacet
    {
        public string _producer { get; set; }
        public string _schemaURL { get; set; }
        public bool _deleted { get; set; }
    }
}
