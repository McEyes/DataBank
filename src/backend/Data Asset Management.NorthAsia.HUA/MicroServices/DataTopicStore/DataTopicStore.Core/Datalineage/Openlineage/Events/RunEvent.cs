using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Enums;
using DataTopicStore.Core.Datalineage.Openlineage.Models;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Openlineage.Events
{
    public class RunEvent
    {
        public static RunEvent CreateInstance(string ns,string jobName)
        {
            var runEvent = new RunEvent
            {
                EventType = Core.Datalineage.Openlineage.Enums.RunState.COMPLETE,
                EventTime = DateTime.UtcNow,
                Run = new Run { RunId = Guid.NewGuid().ToString() },
                Job = new Job
                {
                    Namespace = ns,
                    Name = jobName
                },
                Inputs = new List<InputDataset>(),
                Outputs = new List<OutputDataset>(),
                Producer = "http://cnhuam0webstg85:6002/#/home/page",
                SchemaURL = "http://cnhuam0webstg85:6002/#/home/page",
            };

            return runEvent;
        }

        [JsonProperty("eventType")]
        public RunState EventType { get; set; }
        [JsonProperty("eventTime")]
        public DateTime EventTime { get; set; }
        [JsonProperty("run")]
        public Run Run { get; set; }
        [JsonProperty("job")]
        public Job Job { get; set; }
        [JsonProperty("inputs")]
        public List<InputDataset> Inputs { get; set; }
        [JsonProperty("outputs")]
        public List<OutputDataset> Outputs { get; set; }
        [JsonProperty("producer")]
        public string Producer { get; set; }
        [JsonProperty("schemaURL")]
        public string SchemaURL { get; set; }
    }
}
