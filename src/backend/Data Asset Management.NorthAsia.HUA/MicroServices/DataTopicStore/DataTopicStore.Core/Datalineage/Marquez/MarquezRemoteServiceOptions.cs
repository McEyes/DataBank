using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Marquez
{
    public class MarquezRemoteServiceOptions
    {
        public string BaseUrl { get; set; } = "http://huam0itstg96:3000";
        public string ClientName { get; set; } = "MarquezClient";
        public string LineageUrl { get; set; } = "/api/v1/lineage";
        public string ColumnlineageUrl { get; set; } = "/api/v1/column-lineage?nodeId=dataset:datatopicstore:_default_.{name}&depth=2&withDownstream=true";
    }
}
