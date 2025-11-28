using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Marquez.Models.ColumnLineages
{
    public class ColumnlineageNode
    {
        public string id { get; set; }
        public string type { get; set; }
        public ColumnlineageNode_Data data { get; set; }
    }

    public class ColumnlineageNode_Data
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; }
        public string dataset { get; set; }
        public string field { get; set; }
        public string fieldType { get; set; }
        public List<ColumnlineageNode_Data_InputField> inputFields { get; set; }
    }

    public class ColumnlineageNode_Data_InputField
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; }
        public string dataset { get; set; }
        public string field { get; set; }
    }
}
