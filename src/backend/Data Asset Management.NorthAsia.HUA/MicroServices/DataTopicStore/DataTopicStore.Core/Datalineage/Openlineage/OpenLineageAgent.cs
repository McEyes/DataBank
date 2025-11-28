using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Dtos;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using DataTopicStore.Core.Datalineage.Openlineage.Models;
using Furion;

namespace DataTopicStore.Core.Datalineage.Openlineage
{
    public class OpenLineageAgent
    {
        private OpenLineageClient client;
        public OpenLineageAgent()
        {
            this.client = new OpenLineageClient();
        }

        public async Task<bool> EmitEvent(RunEvent runEvent)
        {
            var options = GetOptions();
            var url = $"{options.BaseUrl}{options.LineageUrl}";
            return await client.EmitEvent(url, runEvent);
        }

        public async Task<bool> CreateDataset(string ns, string dataset, CreateDatasetDto dto)
        {
            var options = GetOptions();
            var url = $"{options.BaseUrl}{options.DatasetUrl.Replace(":namespace", ns).Replace(":dataset", dataset)}";
            return await client.CreateDataset(url, dto);
        }

        public static OpenlineageRemoteServiceOptions GetOptions()
        {
            return App.GetConfig<OpenlineageRemoteServiceOptions>("OpenlineageRemoteServiceOptions");
        }
    }
}