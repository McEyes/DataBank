using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Marquez.Models.ColumnLineages;

namespace DataTopicStore.Core.Datalineage.Marquez.Models
{
    public class LineageGraph
    {
        public List<LineageNode> graph { get; set; }
        public List<LineageNode_Dataset_Item> datasets { get; set; }
        public List<ColumnlineageOutputField> columnlineages { get; set; }
    }
}
