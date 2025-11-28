using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Openlineage.Models.ColumnLineages
{
    public class ColumnLineageFacet
    {
        [JsonProperty("_producer")]
        public string _producer { get; set; }
        [JsonProperty("_schemaURL")]
        public string _schemaURL { get; set; }
        [JsonProperty("fields")]
        public Dictionary<string,object> fields { get; set; }
    }
}
