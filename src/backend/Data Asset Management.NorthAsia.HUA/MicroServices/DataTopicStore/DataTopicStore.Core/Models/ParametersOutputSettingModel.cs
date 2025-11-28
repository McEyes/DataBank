using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Models
{
    public class ParametersOutputItemModel
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
    }

    public class ParametersOutputSettingModel
    {
        /// <summary>
        /// object,array
        /// </summary>
        public string Type { get; set; }
        public bool IsPaged { get; set; }
        public List<ParametersOutputItemModel> Parameters { get; set; }
    }
}
