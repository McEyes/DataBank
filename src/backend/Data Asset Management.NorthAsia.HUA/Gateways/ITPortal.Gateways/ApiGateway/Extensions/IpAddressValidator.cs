using ApiGateway.Models;

using System.Net;
using System.Net.Sockets;

namespace ApiGateway.Extensions
{
    public static class IpAddressValidator
    { 
        // 检查IP是否匹配目标列表（支持精确IP、CIDR段、通配符IP）
        public static bool IsMatch(string clientIp, ParsedAccessList list)
        {
            if (string.IsNullOrEmpty(clientIp)) return false;

            // 1. 匹配精确IP
            if (list.ExactIps.Any(ip => IsIpEqual(clientIp, ip)))
                return true;

            // 2. 匹配CIDR段
            if (list.CidrRanges.Any(range => IsInCidrRange(clientIp, range)))
                return true;

            // 3. 匹配通配符IP（如10.114.*、10.186.16*）
            if (list.WildcardIps.Any(wildcard => IsMatchWildcardIp(clientIp, wildcard)))
                return true;

            return false;
        }

        // 通配符IP匹配逻辑（仅支持IPv4，按.分割的段处理*）
        private static bool IsMatchWildcardIp(string clientIp, string wildcard)
        {
            // 仅处理IPv4（IPv6通配符规则暂不支持，可按需扩展）
            if (!clientIp.Contains(".") || !wildcard.Contains("."))
                return false;

            var clientSegments = clientIp.Split('.');
            var wildcardSegments = wildcard.Split('.');

            // 段数必须一致（如10.114.*是3段，客户端IP必须是4段则不匹配）
            if (clientSegments.Length != wildcardSegments.Length)
                return false;

            for (int i = 0; i < clientSegments.Length; i++)
            {
                var clientSeg = clientSegments[i];
                var wildcardSeg = wildcardSegments[i];

                // 通配符*匹配任意值
                if (wildcardSeg == "*")
                    continue;

                // 通配符前缀（如16*匹配16、160、168等）
                if (wildcardSeg.EndsWith("*"))
                {
                    var prefix = wildcardSeg.TrimEnd('*');
                    if (!clientSeg.StartsWith(prefix))
                        return false;
                }
                // 精确匹配段
                else if (clientSeg != wildcardSeg)
                {
                    return false;
                }
            }
            return true;
        }

        // 检查IP是否在目标列表中（支持单个IP或CIDR段）
        public static bool IsInList(string clientIp, List<string> targetList)
        {
            if (string.IsNullOrEmpty(clientIp) || targetList == null || !targetList.Any())
                return false;

            foreach (var target in targetList)
            {
                if (string.IsNullOrEmpty(target))
                    continue;

                // 目标是单个IP（不含/）
                if (!target.Contains("/"))
                {
                    if (IsIpEqual(clientIp, target))
                        return true;
                }
                // 目标是CIDR段（含/）
                else
                {
                    if (IsInCidrRange(clientIp, target))
                        return true;
                }
            }
            return false;
        }

        // 验证两个IP是否相等（支持IPv4/IPv6）
        private static bool IsIpEqual(string ip1, string ip2)
        {
            return IPAddress.TryParse(ip1, out var addr1)
                && IPAddress.TryParse(ip2, out var addr2)
                && addr1.Equals(addr2);
        }

        // 验证IP是否在CIDR段中
        private static bool IsInCidrRange(string clientIp, string cidrRange)
        {
            if (!IPAddress.TryParse(clientIp, out var ip))
                return false;

            var parts = cidrRange.Split('/');
            if (parts.Length != 2)
                return false;

            if (!IPAddress.TryParse(parts[0], out var networkAddress))
                return false;

            if (!int.TryParse(parts[1], out var prefixLength) || prefixLength < 0)
                return false;

            // 确保IP版本一致
            if (ip.AddressFamily != networkAddress.AddressFamily)
                return false;

            return ip.AddressFamily switch
            {
                AddressFamily.InterNetwork => IsInRangeIPv4(ip, networkAddress, prefixLength),
                AddressFamily.InterNetworkV6 => IsInRangeIPv6(ip, networkAddress, prefixLength),
                _ => false
            };
        }

        // IPv4段验证
        private static bool IsInRangeIPv4(IPAddress ip, IPAddress network, int prefixLength)
        {
            var ipBytes = ip.GetAddressBytes();
            var networkBytes = network.GetAddressBytes();
            var mask = GetIPv4Mask(prefixLength);

            for (int i = 0; i < 4; i++)
            {
                if ((ipBytes[i] & mask[i]) != (networkBytes[i] & mask[i]))
                    return false;
            }
            return true;
        }

        // IPv6段验证
        private static bool IsInRangeIPv6(IPAddress ip, IPAddress network, int prefixLength)
        {
            var ipBytes = ip.GetAddressBytes();
            var networkBytes = network.GetAddressBytes();
            int fullBytes = prefixLength / 8;
            int remainingBits = prefixLength % 8;

            for (int i = 0; i < fullBytes; i++)
            {
                if (ipBytes[i] != networkBytes[i])
                    return false;
            }

            if (remainingBits > 0)
            {
                byte mask = (byte)(0xFF << (8 - remainingBits));
                if ((ipBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
                    return false;
            }

            return true;
        }

        // 获取IPv4子网掩码
        private static byte[] GetIPv4Mask(int prefixLength)
        {
            var mask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                int bits = Math.Min(8, prefixLength);
                mask[i] = (byte)(0xFF << (8 - bits));
                prefixLength -= bits;
                if (prefixLength <= 0)
                    break;
            }
            return mask;
        }
    }
}
