using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Openlineage.Models
{
    public class FacetsSchema
    {
        [JsonProperty("_producer")]
        public string _Producer { get; set; }

        [JsonProperty("_schemaURL")]
        public string _SchemaURL { get; set; }
    }
}
