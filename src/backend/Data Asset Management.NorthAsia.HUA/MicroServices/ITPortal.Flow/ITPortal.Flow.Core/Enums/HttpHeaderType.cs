using System.ComponentModel;

namespace ITPortal.Flow.Core
{

    /// <summary>
    /// 
    /// </summary>
    public enum HttpHeaderType
    {
        [Description("Accept")]
        Accept = 0,
        [Description("Accept-Charset")]
        AcceptCharset = 1,
        [Description("Accept-Language")]
        AcceptLanguage = 2,
        [Description("Accept-DateTimeOffset")]
        AcceptDatetime = 3,

        [Description("Authorization")]
        Authorization = 4,
        [Description("Cache-Control")]
        CacheControl = 5,
        [Description("Connection")]
        Connection = 6,
        [Description("Cookie")]
        Cookie = 7,
        [Description("Content-Length")]
        ContentLength = 8,
        [Description("Content-MD5")]
        ContentMD5 = 9,
        [Description("Content-Type")]
        ContentType = 10,
        [Description("Date")]
        Date = 11,

        [Description("Expect")]
        Expect = 12,
        [Description("From")]
        From = 13,
        [Description("Host")]
        Host = 14,
        [Description("Origin")]
        Origin = 15,
        [Description("Proxy-Authorization")]
        ProxyAuthorization = 16,
        [Description("User-Agent")]
        UserAgent = 17,
        [Description("Warning")]
        Warning = 18,
    }
}
