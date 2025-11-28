using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Dtos;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataTopicStore.Core.Datalineage.Openlineage
{
    public class OpenLineageClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _openLineageUrl;

        public OpenLineageClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> EmitEvent(string url,RunEvent runEvent)
        {
            var json = JsonConvert.SerializeObject(runEvent, new StringEnumConverter());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode) { return true; }
            else
            {
                var results = await response.Content.ReadAsStringAsync();
                throw new Exception(results);
            }
        }

        public async Task<bool> CreateDataset(string url,CreateDatasetDto dto)
        {
            var json = JsonConvert.SerializeObject(dto, new StringEnumConverter());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            if (response.IsSuccessStatusCode) { return true; }
            else
            {
                var results = await response.Content.ReadAsStringAsync();
                throw new Exception(results);
            }
        }
    }
}
