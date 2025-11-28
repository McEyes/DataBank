using System.ComponentModel;

namespace ITPortal.Flow.Core
{

    /// <summary>
    /// 
    /// </summary>
    public enum HttpAuthType
    {
        [Description("No Auth")]
        NoAuth = 0,
        [Description("Basic Auth")]
        Basic = 1,
        [Description("Bearer Token")]
        BearerToken = 2,
    }
}
