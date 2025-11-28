using ApiGateway.Models;

using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace ApiGateway.Extensions
{
    public static class MachineNameValidator
    {
        // 检查机器名是否匹配目标列表（支持精确匹配、模糊匹配）
        public static bool IsMatch(string machineName, ParsedAccessList list)
        {
            if (string.IsNullOrEmpty(machineName)) return false;

            // 1. 精确匹配
            if (list.ExactMachineNames.Contains(machineName, StringComparer.OrdinalIgnoreCase))
                return true;

            // 2. 模糊匹配（支持*通配符）
            foreach (var wildcard in list.WildcardMachineNames)
            {
                if (IsMatchWildcard(machineName, wildcard))
                    return true;
            }

            return false;
        }

        // 模糊匹配逻辑（*表示任意字符）
        private static bool IsMatchWildcard(string input, string wildcard)
        {
            // 转换为正则表达式（* → .*）
            var pattern = "^" + Regex.Escape(wildcard).Replace("\\*", ".*") + "$";
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }
    }
}
