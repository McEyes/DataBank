using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Furion.LinqBuilder;
using DataTopicStore.Core.Datalineage.Marquez.Models;

namespace DataTopicStore.Core.Datalineage.Marquez
{
    public class MarquezClient
    {
        private readonly HttpClient _httpClient;
        //private readonly string _marquazUrl;

        public MarquezClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetLineageDataAsync(string openLineageUrl, string nodeId)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nodeId);

            var url = $"{openLineageUrl}?nodeId={nodeId}&depth=2";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var results = await response.Content.ReadAsStringAsync();
                throw new Exception(results);
            }
        }

        public async Task<string> GetColumnLineageDataAsync(string columnLineageUrl, string nodeId)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nodeId);

            var url = $"{columnLineageUrl}?nodeId={nodeId}&depth=2";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var results = await response.Content.ReadAsStringAsync();
                throw new Exception(results);
            }
        }
    }
}
