using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTopicStore.Core.Datalineage.Marquez.Models.ColumnLineages
{
    public class ColumnlineageOutputField
    {
        public string name { get; set; }

        public List<ColumnlineageInputField> inputFields { get; set; }
    }
}
