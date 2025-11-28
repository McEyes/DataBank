using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataTopicStore.Core.Enums
{
    public enum ProgressStatus : byte
    {
        BusinessModeling = 1,
        ITDeveloping = 2,
        Validation = 3,
        Publish = 9,
        Completed = 81
    }
}