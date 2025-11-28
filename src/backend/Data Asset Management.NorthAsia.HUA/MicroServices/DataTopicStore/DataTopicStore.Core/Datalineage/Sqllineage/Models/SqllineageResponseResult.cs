using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Sqllineage.Models
{
    public class SqllineageResponseResult
    {
        public int StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string Data { get; set; }
        public string Errors { get; set; }
    }
}
