using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using DataTopicStore.Core.Datalineage.Sqllineage.Models;
using DataTopicStore.Core.Datalineage.Sqllineage.Parsers;
using System.Text.RegularExpressions;

namespace DataTopicStore.Core.Datalineage.Sqllineage
{
    public class SqllineageClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _sqllineageUrl;

        public SqllineageClient(string sqllineageUrl)
        {
            _httpClient = new HttpClient();
            _sqllineageUrl = sqllineageUrl;
        }

        private string ReplaceSymbol(string sql)
        {
            Regex regex = new Regex("@[\\w]+", RegexOptions.IgnoreCase);
            var results = regex.Replace(sql, "0");
            return results;
        }

        public async Task<SqllineageTableResults> ExtractTables(string sql,bool column = false)
        {
            sql = ReplaceSymbol(sql);
            var dto = new SqllineageCmdText { Base64CmdText = Convert.ToBase64String(Encoding.UTF8.GetBytes($"sqllineage -e \"{sql}\"")) };
            var json = JsonConvert.SerializeObject(dto, new StringEnumConverter());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_sqllineageUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                SqllineageResponseResult responseResult = JsonConvert.DeserializeObject<SqllineageResponseResult>(results);
                if (responseResult.Succeeded)
                    return TablesParser.Extract(responseResult.Data);
                return null;
            }
            else
            {
                var results = await response.Content.ReadAsStringAsync();
                throw new Exception(results);
            }
        }

        public async Task<Dictionary<string,object>> ExtractColumnLineages(string sql)
        {
            sql = ReplaceSymbol(sql);
            var dto = new SqllineageCmdText { Base64CmdText = Convert.ToBase64String(Encoding.UTF8.GetBytes($"sqllineage -e \"{sql}\" -l column")) };
            var json = JsonConvert.SerializeObject(dto, new StringEnumConverter());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_sqllineageUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                SqllineageResponseResult responseResult = JsonConvert.DeserializeObject<SqllineageResponseResult>(results);
                if (responseResult.Succeeded)
                    return ColumnLineageParser.Extract(responseResult.Data);
                return null;
            }
            else
            {
                var results = await response.Content.ReadAsStringAsync();
                throw new Exception(results);
            }
        }
    }
}
