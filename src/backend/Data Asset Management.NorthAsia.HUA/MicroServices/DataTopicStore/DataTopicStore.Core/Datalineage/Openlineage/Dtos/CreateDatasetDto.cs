using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Openlineage.Dtos
{
    public class CreateDatasetDto
    {
        public string type { get; set; }
        public string physicalName { get; set; }
        public string sourceName { get; set; }
        public string description { get; set; }
        public List<LineageNode_Data_Field> fields { get; set; }

        public class LineageNode_Data_Field
        {
            public string type { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
    }
}
