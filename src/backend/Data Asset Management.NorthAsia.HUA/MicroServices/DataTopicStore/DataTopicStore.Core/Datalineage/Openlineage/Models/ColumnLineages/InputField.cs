using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Openlineage.Models.ColumnLineages
{
    public class InputField
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("field")]
        public string field { get; set; }
    }
}
