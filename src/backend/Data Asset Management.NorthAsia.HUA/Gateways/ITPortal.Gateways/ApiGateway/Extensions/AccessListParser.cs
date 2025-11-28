using ApiGateway.Models;

using System.Net;
using System.Net.Sockets;

namespace ApiGateway.Extensions
{
    public static class AccessListParser
    {
        public static ParsedAccessList Parse(List<string> mixedList)
        {
            var result = new ParsedAccessList();
            if (mixedList == null) return result;

            foreach (var value in mixedList)
            {
                var trimmed = value.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                // 1. 识别CIDR段（含/）
                if (trimmed.Contains("/"))
                {
                    result.CidrRanges.Add(trimmed);
                }
                // 2. 识别IP相关规则（含.或:，可能是精确IP或通配符IP）
                else if (trimmed.Contains(".") || trimmed.Contains(":"))
                {
                    if (trimmed.Contains("*")) // 通配符IP（如10.114.*）
                    {
                        result.WildcardIps.Add(trimmed);
                    }
                    else if (IsValidIpAddress(trimmed)) // 精确IP（如192.168.1.100）
                    {
                        result.ExactIps.Add(trimmed);
                    }
                    else // 无效IP格式（视为机器名，避免误判）
                    {
                        AddMachineNameRule(trimmed, result);
                    }
                }
                // 3. 机器名规则（不含.和:）
                else
                {
                    AddMachineNameRule(trimmed, result);
                }
            }
            return result;
        }

        // 区分精确机器名和模糊机器名（含*）
        private static void AddMachineNameRule(string value, ParsedAccessList result)
        {
            if (value.Contains("*"))
                result.WildcardMachineNames.Add(value); // 模糊匹配
            else
                result.ExactMachineNames.Add(value); // 精确匹配
        }

        // 验证是否为精确IP（不含*）
        private static bool IsValidIpAddress(string value)
        {
            return IPAddress.TryParse(value, out _);
        }
    }
}
