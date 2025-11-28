using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Marquez.Models.ColumnLineages
{
    public class ColumnlineageInputField
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        public string dataset { get; set; }

        public string field { get; set; }
    }
}
