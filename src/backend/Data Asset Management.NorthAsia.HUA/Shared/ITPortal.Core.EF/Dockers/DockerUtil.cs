using ITPortal.Extension.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Encrypt
{
    public class DockerUtil
    {
        /// <summary>
        ///  获取docker的网络ip
        /// </summary>
        /// <param name="ipOrName">=0获取ip，1获取带域的服务器名称，2获取服务器名称</param>
        /// <returns></returns>
        public static string GetHostIpFromEtcHosts(int ipOrName=0)
        {
            try
            {
                if(!File.Exists("/tmp/hosts")) return string.Empty;
                var lines = File.ReadAllLines("/tmp/hosts");
                foreach (var line in lines)
                {
                    Console.WriteLine($"read /tmp/hosts :{line}");
                    if (line.Contains("host-gateway") || line.Contains("host.docker.internal") || line.Contains("corp.jabil.org"))
                    {
                        var parts = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1 && IPAddress.TryParse(parts[0], out _))
                        {
                            if (ipOrName == 0)
                                return parts[0];
                            else return parts[ipOrName];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取tmp/hosts失败,read fial:{ex.Message}");
            }
            return string.Empty;
        }

        public static string GetHostIp()
        {
            try
            {
                var name = Dns.GetHostName(); // get container id
                var addressList = Dns.GetHostEntry(name).AddressList;
                foreach (var address in addressList) Console.WriteLine($"read address :{address}, AddressFamily：{address.AddressFamily}");
                var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                return ip.MapToIPv4().ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取GetHostEntry失败,read fial:{ex.Message}");
            }
            return string.Empty;
        }

        public static string GetHostIp6()
        {
            var name = Dns.GetHostName(); // get container id
            var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            return ip.MapToIPv6().ToString();
        }

        public static string GetHostOrIp()
        {
            var hostName = GetHostIpFromEtcHosts();
            if (hostName.IsNullOrWhiteSpace()) hostName = GetHostIp();
            if (hostName.IsNullOrWhiteSpace()) hostName = "localhost";
            return hostName;
        }
        public static string GetHostName()
        {
            var hostName = GetHostIpFromEtcHosts(2);
            if (hostName.IsNullOrWhiteSpace()) hostName = GetHostIp();
            if (hostName.IsNullOrWhiteSpace()) hostName = "localhost";
            return hostName;
        }
    }
}
