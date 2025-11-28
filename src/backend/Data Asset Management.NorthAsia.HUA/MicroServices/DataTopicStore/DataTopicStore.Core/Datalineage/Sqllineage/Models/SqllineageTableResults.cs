using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Sqllineage.Models
{
    public class SqllineageTableResults
    {
        public List<string> SourceTables { get; set; }
        public List<string> TargetTables { get; set; }
        public List<string> IntermediateTables { get; set; }
    }
}
