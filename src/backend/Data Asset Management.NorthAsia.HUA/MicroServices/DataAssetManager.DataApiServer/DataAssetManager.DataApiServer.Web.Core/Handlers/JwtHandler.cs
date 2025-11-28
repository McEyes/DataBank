using ITPortal.Core;
using ITPortal.Core.Services;

using Furion;
using Furion.Authorization;
using Furion.DataEncryption;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Web.Core
{
    public class JwtHandler : AppAuthorizeHandler
    {
        private ILogger<JwtHandler> _logger;
        public JwtHandler(ILogger<JwtHandler> logger) { _logger = logger; }
        /// <summary>
        /// 重写 Handler 添加自动刷新收取逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override async Task HandleAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {
            var cont = context.GetCurrentHttpContext();
            // 自动刷新 token
            if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext()))
            {
                await AuthorizeHandleAsync(context);
            }
            else
            {
                _logger.LogError($"验证权限失败：{httpContext.Request.Path}");
                context.Fail();    // 授权失败
            }
        }

        public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {
            if (!httpContext.Items.ContainsKey(DataAssetManagerConst.HttpContext_UserInfo))
                httpContext.Items.Add(DataAssetManagerConst.HttpContext_UserInfo, new UserInfo(httpContext.User));
            // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false
            var result = CheckAuthorzie(httpContext);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static bool CheckAuthorzie(DefaultHttpContext httpContext)
        {
            // 获取权限特性
            var securityDefineAttribute = httpContext.GetMetadata<SecurityDefineAttribute>();
            if (securityDefineAttribute == null) return true;

            return true;// "查询数据库返回是否有权限";
        }
    }
}
