using System.ComponentModel;

namespace ITPortal.Core.ProxyApi.Flow.Enums
{

    /// <summary>
    /// 
    /// </summary>
    public enum HttpBodyType
    {
        [Description("form-data")]
        FormData = 0,
        [Description(@"x-www-form-urlencoded")]
        FormUrlEncoded = 1,
        [Description("Json")]
        Json = 2,
        [Description("Xml")]
        Xml = 3,
        //[Description("Row")]
        //Row = 4,
    }
}
