using System.Net;
using System.Net.Sockets;

namespace ApiGateway.Extensions
{
    public static class IpAddressRangeValidator
    {
        public static bool IsInRange(string ipAddress, string cidrRange)
        {
            if (!IPAddress.TryParse(ipAddress, out var ip))
                return false;

            var parts = cidrRange.Split('/');
            if (parts.Length != 2)
                return false;

            if (!IPAddress.TryParse(parts[0], out var networkAddress))
                return false;

            if (!int.TryParse(parts[1], out var prefixLength))
                return false;

            // 确保两个地址版本相同
            if (ip.AddressFamily != networkAddress.AddressFamily)
                return false;

            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return IsInRangeIPv4(ip, networkAddress, prefixLength);
            }
            else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return IsInRangeIPv6(ip, networkAddress, prefixLength);
            }

            return false;
        }

        private static bool IsInRangeIPv4(IPAddress ip, IPAddress networkAddress, int prefixLength)
        {
            var ipBytes = ip.GetAddressBytes();
            var networkBytes = networkAddress.GetAddressBytes();
            var maskBytes = GetMaskBytesIPv4(prefixLength);

            for (int i = 0; i < 4; i++)
            {
                if ((ipBytes[i] & maskBytes[i]) != (networkBytes[i] & maskBytes[i]))
                    return false;
            }

            return true;
        }

        private static bool IsInRangeIPv6(IPAddress ip, IPAddress networkAddress, int prefixLength)
        {
            var ipBytes = ip.GetAddressBytes();
            var networkBytes = networkAddress.GetAddressBytes();
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

        private static byte[] GetMaskBytesIPv4(int prefixLength)
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
