using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Openlineage.Models
{
    public class Run
    {
        [JsonProperty("runId")]
        public string RunId { get; set; }

        [JsonProperty("facets")]
        public Dictionary<string, object> Facets { get; set; }
    }
}
