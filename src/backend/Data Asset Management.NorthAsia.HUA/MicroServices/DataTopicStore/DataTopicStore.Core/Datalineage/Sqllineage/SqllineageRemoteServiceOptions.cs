using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Sqllineage
{
    public class SqllineageRemoteServiceOptions
    {
        public string BaseUrl { get; set; } = "http://huam0itstg96:8812";
        public string ClientName { get; set; } = "SqllineageClient";
        public string SqllineageUrl { get; set; } = "/sqllineage";
        //public string ColumnlineageUrl { get; set; } = "/sqllineage";
    }
}
