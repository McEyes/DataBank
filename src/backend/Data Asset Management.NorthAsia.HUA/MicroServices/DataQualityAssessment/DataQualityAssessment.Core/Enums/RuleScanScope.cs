using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Enums
{
    public enum RuleScanScope : byte
    {
        [Description("整个表的所有列")]
        AllTable = 0,
        [Description("指定列")]
        OneField = 1,
        [Description("所有日期类型的列")]
        DateType = 2,
        [Description("所有枚举类型的列")]
        EnumType = 3
    }
}
