using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Models
{
    public class ParametersInputItemModel
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public string Format { get; set; }
    }

    public class ParametersInputSettingModel
    {
        public List<ParametersInputItemModel> Parameters { get; set; }
    }
}
