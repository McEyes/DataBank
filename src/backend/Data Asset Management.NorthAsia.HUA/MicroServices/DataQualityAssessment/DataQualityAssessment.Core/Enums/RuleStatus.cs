using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Enums
{
    public enum RuleStatus : byte
    {
        [Description("草稿")]
        Draft = 0,
        [Description("已发布")]
        Publish = 1
    }
}
