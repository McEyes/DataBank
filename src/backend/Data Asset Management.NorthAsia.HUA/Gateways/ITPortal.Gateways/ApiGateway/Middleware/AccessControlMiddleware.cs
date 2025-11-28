using ApiGateway.Extensions;
using ApiGateway.Models;

namespace ApiGateway.Middleware
{
    public class AccessControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ParsedAccessList _parsedWhiteList; // 解析后的白名单
        private readonly ParsedAccessList _parsedBlackList; // 解析后的黑名单
        private readonly string _redirectUrl;

        public AccessControlMiddleware(
            RequestDelegate next,
            AccessControlSettings settings)
        {
            _next = next;
            _redirectUrl = settings.RedirectUrl;

            // 初始化时解析白名单和黑名单（仅执行一次）
            _parsedWhiteList = AccessListParser.Parse(settings.WhiteList);
            _parsedBlackList = AccessListParser.Parse(settings.BlackList);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 获取客户端IP（兼容反向代理）
            var clientIp = GetClientIp(context);
            // 获取客户端机器名（示例：从自定义头获取）
            var machineName = context.Request.Headers["X-Machine-Name"].FirstOrDefault()?.Trim() ?? string.Empty;

            // 检查是否需要拦截
            if (ShouldBlockRequest(clientIp, machineName))
            {
                context.Response.Redirect(_redirectUrl);
                return;
            }

            // 允许访问，继续处理请求
            await _next(context);
        }

        // 核心拦截逻辑
        private bool ShouldBlockRequest(string? clientIp, string machineName)
        {
            // 1. 客户端IP为空 → 拦截
            if (string.IsNullOrEmpty(clientIp))
                return true;

            // 2. 白名单验证（机器名 → IP）
            if (MachineNameValidator.IsMatch(machineName, _parsedWhiteList))
                return false;

            if (IpAddressValidator.IsMatch(clientIp, _parsedWhiteList))
                return false;

            // 3. 黑名单验证（机器名 → IP）
            if (MachineNameValidator.IsMatch(machineName, _parsedBlackList))
                return true;

            if (IpAddressValidator.IsMatch(clientIp, _parsedBlackList))
                return true;

            // 4. 默认拦截
            return true;

            //// 2. 白名单优先：匹配白名单中的机器名 → 允许访问
            //if (_parsedWhiteList.MachineNames.Contains(machineName, StringComparer.OrdinalIgnoreCase))
            //    return false;

            //// 3. 白名单IP验证：匹配单个IP或IP段 → 允许访问
            //bool isInWhiteIp = IpAddressValidator.IsInList(clientIp, _parsedWhiteList.Ips);
            //bool isInWhiteRange = IpAddressValidator.IsInList(clientIp, _parsedWhiteList.IpRanges);
            //if (isInWhiteIp || isInWhiteRange)
            //    return false;

            //// 4. 黑名单验证：匹配机器名 → 拦截
            //if (_parsedBlackList.MachineNames.Contains(machineName, StringComparer.OrdinalIgnoreCase))
            //    return true;

            //// 5. 黑名单IP验证：匹配单个IP或IP段 → 拦截
            //bool isInBlackIp = IpAddressValidator.IsInList(clientIp, _parsedBlackList.Ips);
            //bool isInBlackRange = IpAddressValidator.IsInList(clientIp, _parsedBlackList.IpRanges);
            //if (isInBlackIp || isInBlackRange)
            //    return true;

            //// 6. 未匹配任何规则 → 默认拦截（可改为默认允许，返回false即可）
            //return true;
        }

        // 获取真实客户端IP（处理反向代理场景）
        private string? GetClientIp(HttpContext context)
        {
            // 优先从反向代理头获取（如Nginx的X-Forwarded-For）
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For格式：客户端IP, 代理1IP, 代理2IP → 取第一个
                return forwardedFor.Split(',', StringSplitOptions.TrimEntries).FirstOrDefault();
            }

            // 非代理场景：直接获取连接IP
            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
