using Elastic.Clients.Elasticsearch;

using Furion;
using Furion.DependencyInjection;
using Furion.HttpRemote;

using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.ProxyApi
{

    /// <summary>
    /// user 信息接口代理
    /// </summary>
    public class BaseProxyService// : ITransient //(IHttpRemoteService httpRemoteService)
    {
        public readonly IHttpRemoteService httpRemoteService;
        public BaseProxyService(IHttpRemoteService httpRemoteService)
        {
            this.httpRemoteService = httpRemoteService;
        }

        /// <summary>
        /// 获取当前授权token
        /// </summary>
        /// <returns></returns>
        public virtual string GetToken()
        {
            var token = string.Empty;
            if (App.HttpContext != null && App.HttpContext.Request.Headers != null)
                token = App.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(token)) token = token.Replace("Bearer ", "");
            return token;
        }
        /// <summary>
        /// 获取dataseet专门用于内部访问api的账号权限
        /// </summary>
        /// <param name="ntid"></param>
        /// <returns></returns>
        protected virtual string GetDataAssetApiToken(string userName)
        {
            return JwtHelper.GenerateJwtToken(new UserInfo()
            {
                Id = userName,
                UserId = userName,
                UserName = userName,
                Name = userName,
                NtId = userName,
                Surname = userName,
            });
        }
    }
}
