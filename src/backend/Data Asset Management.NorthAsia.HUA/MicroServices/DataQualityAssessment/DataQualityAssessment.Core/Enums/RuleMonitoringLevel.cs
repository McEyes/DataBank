using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Enums
{
    public enum RuleMonitoringLevel: byte
    {
        [Description("未知")]
        UnKnown = 0,
        [Description("表级别")]
        Table = 1,
        [Description("字段级别")]
        Field = 2
    }
}