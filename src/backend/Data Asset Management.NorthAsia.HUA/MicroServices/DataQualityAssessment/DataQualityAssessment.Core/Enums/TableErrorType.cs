using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Enums
{
    public enum TableErrorType : byte
    {
        [Description("UnKnown")]
        UnKnown = 0,
        [Description("Unknown table")]
        UnknownTable = 1,
        [Description("Unknown column")]
        UnknownColumn = 2,
        [Description("sql hits sql block rule")]
        SqlBlockRule = 3,
        [Description("Timeout")]
        ConnectionTimeout = 4,
        [Description("Unable to connect")]
        UnableToConnectServer = 5,
    }
}
