using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Openlineage.Enums
{
    public enum RunState
    {
        START,
        RUNNING,
        COMPLETE,
        ABORT,
        FAIL,
        OTHER
    }
}
