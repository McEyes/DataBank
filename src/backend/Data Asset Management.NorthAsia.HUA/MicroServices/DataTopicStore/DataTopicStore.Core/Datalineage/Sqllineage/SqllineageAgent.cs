using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using DataTopicStore.Core.Datalineage.Openlineage;
using Furion;
using DataTopicStore.Core.Datalineage.Sqllineage.Models;

namespace DataTopicStore.Core.Datalineage.Sqllineage
{
    public class SqllineageAgent
    {
        public SqllineageAgent()
        {
        }

        public async Task<SqllineageTableResults> ExtractTables(string sql)
        {
            var options = GetOptions();
            var url = $"{options.BaseUrl}{options.SqllineageUrl}";
            var client = new SqllineageClient(url);
            return await client.ExtractTables(sql);
        }

        public async Task<Dictionary<string, object>> ExtractColumnLineages(string sql)
        {
            var options = GetOptions();
            var url = $"{options.BaseUrl}{options.SqllineageUrl}";
            var client = new SqllineageClient(url);
            return await client.ExtractColumnLineages(sql);
        }

        public static SqllineageRemoteServiceOptions GetOptions()
        {
            return App.GetConfig<SqllineageRemoteServiceOptions>("SqllineageRemoteServiceOptions");
        }
    }
}
