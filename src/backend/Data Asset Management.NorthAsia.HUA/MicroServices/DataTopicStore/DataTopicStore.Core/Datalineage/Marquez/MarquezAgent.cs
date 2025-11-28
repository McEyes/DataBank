using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using DataTopicStore.Core.Datalineage.Openlineage;
using Furion;
using DataTopicStore.Core.Datalineage.Marquez.Models;
using System.Xml.Linq;
using Newtonsoft.Json;
using Mapster;
using DataTopicStore.Core.Datalineage.Marquez.Models.ColumnLineages;
using SqlSugar;

namespace DataTopicStore.Core.Datalineage.Marquez
{
    public class MarquezAgent
    {
        private MarquezClient client;
        public MarquezAgent()
        {
            this.client = new MarquezClient();
        }

        public async Task<LineageGraph> GetLineageDataAsync(string ns, string name)
        {
            var options = GetOptions();
            var url = $"{options.BaseUrl}{options.LineageUrl}";
            ns = ns ?? "datatopicstore";
            var nodeId = $"job:{ns}:{name}";
            var data = await client.GetLineageDataAsync(url,nodeId);
            if (!string.IsNullOrWhiteSpace(data))
            {
                return JsonConvert.DeserializeObject<LineageGraph>(data);
            }
            return null;
        }

        public async Task<ColumnlineageGraph> GetColumnLineageDataAsync(string ns, string name)
        {
            var options = GetOptions();
            var url = $"{options.BaseUrl}{options.ColumnlineageUrl.Replace("{name}",name)}";
            ns = ns ?? "datatopicstore";
            var nodeId = $"dataset:{ns}:_default_.{name}";
            var data = await client.GetColumnLineageDataAsync(url,nodeId);
            if (!string.IsNullOrWhiteSpace(data))
            {
                return JsonConvert.DeserializeObject<ColumnlineageGraph>(data);
            }
            return null;
        }

        public async Task<LineageGraph> GetLineageDataAsync(string name)
        {
            var results = await this.GetLineageDataAsync(ns: null, name: name);
            var graph = new LineageGraph();
            if (results != null)
            {
                var nodeId = $"job:datatopicstore:{name}";
                graph.graph = results.graph.Where(t => t.id == nodeId).ToList();
                graph.datasets = results.graph.Where(t => t.type == "DATASET").Select(t=>t.data.Adapt<LineageNode_Dataset_Item>()).ToList();
                graph.columnlineages = await GetColumnLineageDataAsync(name);
            }

            return graph;
        }

        public async Task<List<ColumnlineageOutputField>> GetColumnLineageDataAsync(string name)
        {
            var results = await this.GetColumnLineageDataAsync(ns: null, name: name);
            var graph = new List<ColumnlineageOutputField>();
            if (results != null && results.graph.Count > 0)
            {
                graph = results.graph.Where(t => t.type == "DATASET_FIELD" && t.data.inputFields.Count > 0).Select(t => new ColumnlineageOutputField
                {
                    name = t.data.field,
                    inputFields = t.data.inputFields.Select(a => a.Adapt<ColumnlineageInputField>()).ToList()
                }).ToList();
            }

            return graph;
        }

        public static MarquezRemoteServiceOptions GetOptions()
        {
            return App.GetConfig<MarquezRemoteServiceOptions>("MarquezRemoteServiceOptions");
        }
    }
}
