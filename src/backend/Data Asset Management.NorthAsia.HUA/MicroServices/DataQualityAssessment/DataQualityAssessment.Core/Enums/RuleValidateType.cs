using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Enums
{
    public enum RuleValidateType : byte
    {
        [Description("邮箱验证")]
        Email = 0,
        [Description("手机号码")]
        PhoneNumber = 1,
        [Description("空值比例")]
        NullValueRatio = 2,
        [Description("时间时效性")]
        Timeliness = 3,
        [Description("多余字段占比")]
        ExtraColumns = 4,
        [Description("数据类型验证_日期类型")]
        DataTypeDateType = 5,
        [Description("数据类型验证_枚举类型")]
        DataTypeEnumType = 6,
        [Description("数据可解释性_数据字典完整性")]
        DataInterpretability = 7,
        [Description("数据类型验证_VARCHAR类型")]
        DataTypeVarcharType = 8,
        [Description("数据信息的完整性")]
        DataInformationIntegrity = 9,
    }
}
