using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Enums
{
    public enum DatabaseType : byte
    {
        [Description("MSSqlServer")]
        SqlServer = 0,
        [Description("Mysql")]
        Mysql = 1,
        [Description("Postgresql")]
        Postgresql = 2
    }
}
