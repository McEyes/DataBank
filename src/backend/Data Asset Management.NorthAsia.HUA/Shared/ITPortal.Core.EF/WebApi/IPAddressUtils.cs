using ITPortal.Extension.System;

using Microsoft.AspNetCore.Http;

using System.Net;

namespace ITPortal.Core.WebApi
{
    public class IPAddressUtils
    {
        public static string GetClientIp(HttpRequest request)
        {
            if (request.HttpContext.Items.TryGetValue("Remote-IP", out object ipObj))
            {
                return ipObj.ToString();
            }
            var headers = new List<string>() { "X-Forwarded-For", "Proxy-Client-IP", "WL-Proxy-Client-IP", "HTTP_CLIENT_IP", "HTTP_X_FORWARDED_FOR", "X-Forwarded-For" };
            var ip = GetAndCheckIP(request, headers.GetEnumerator());
            if (string.IsNullOrWhiteSpace(ip)) ip = request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();
            if (ip.IsNullOrWhiteSpace()) ip = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            request.HttpContext.Items.TryAdd("Remote-IP", ip);
            return ip;
        }

        private static string GetAndCheckIP(HttpRequest request, IEnumerator<string> headerNames)
        {
            while (headerNames.MoveNext())
            {
                var xffHeader = request.Headers[headerNames.Current].FirstOrDefault();
                if (!string.IsNullOrEmpty(xffHeader))
                {
                    var remoteIp = xffHeader.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (IPAddress.TryParse(remoteIp, out var ip))
                    {
                        return ip.ToString();
                    }
                }
            }
            return string.Empty;
        }
    }
}
