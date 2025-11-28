using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Openlineage
{
    public class OpenlineageRemoteServiceOptions
    {
        public string BaseUrl { get; set; } = "http://huam0itstg96:5000";
        public string VisableUrl { get; set; } = "http://huam0itstg96:3000";
        public string ClientName { get; set; } = "OpenlineageClient";
        public string LineageUrl { get; set; } = "/api/v1/lineage";
        public string DatasetUrl { get; set; } = "/api/v1/namespaces/:namespace/datasets/:dataset";
    }
}
