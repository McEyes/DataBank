using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Marquez.Models
{
    public class LineageNode
    {
        public string id { get; set; }
        public string type { get; set; }
        public LineageNode_Data data { get; set; }

        public class LineageNode_Data
        {
            public LineageNode_Data_Item id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string @namespace { get; set; }
            public List<LineageNode_Data_Input> inputs { get; set; }
            public List<LineageNode_Data_Output> outputs { get; set; }

            //type=DATASET
            public List<LineageNode_Dataset_Field_Item> fields { get; set; }
        }

        public class LineageNode_Data_Input
        {
            public string Namespace { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
        public class LineageNode_Data_Output
        {
            public string Namespace { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
        public class LineageNode_Data_Item
        {
            public string Namespace { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
    }

    public class LineageNode_Dataset_Item
    {
        public string Namespace { get; set; }
        public string sourceName { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<LineageNode_Dataset_Field_Item> fields { get; set; }
    }

    public class LineageNode_Dataset_Field_Item
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<string> tags { get; set; }
        public string description { get; set; }
    }
}
