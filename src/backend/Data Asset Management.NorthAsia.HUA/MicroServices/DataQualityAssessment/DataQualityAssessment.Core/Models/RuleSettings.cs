using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Core.Models
{
    public class RuleSettings
    {
        public string RuleNo { get; set; }
        public int Weight { get; set; }
        public RuleScanScope ScanScope { get; set; }
        public MetadataTableModel Table { get; set; }
        public List<MetadataColumnModel> Columns { get; set; }
    }
}
