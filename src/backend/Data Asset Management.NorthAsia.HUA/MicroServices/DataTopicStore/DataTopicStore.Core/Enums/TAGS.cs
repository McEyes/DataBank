using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Enums
{
    [Flags]
    public enum TAGS
    {
        KPI = 1,
        Plant = 2,
        Realtime = 4
    }
}
