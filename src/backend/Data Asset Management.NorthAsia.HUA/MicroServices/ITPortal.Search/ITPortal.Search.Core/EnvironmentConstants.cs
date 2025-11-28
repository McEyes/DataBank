using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Core
{
    public static class EnvironmentConstants
    {

#if DEBUG
        public const string Environment = "DEVELOPMENT";
#elif PRODUCTION
        public const string Environment = "PRODUCTION";
#elif STG
        public const string Environment = "STG";
#endif



    }
}
