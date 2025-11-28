using Elastic.Clients.Elasticsearch.MachineLearning;

using Furion;
using Furion.DependencyInjection;
using Furion.HttpRemote;
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
    public class TopicDocumentProxyService : BaseProxyService //(IHttpRemoteService httpRemoteService)
    {
        private readonly string BaseHostUrl;
        public TopicDocumentProxyService(IHttpRemoteService httpRemoteService):base(httpRemoteService) 
        {
            BaseHostUrl = App.GetConfig<string>("RemoteApi:SearchTopicUrl");
        }


        /// <summary>
        /// 开放搜索接口，IT Portal的全局搜索
        /// 实际请求接口：/api/TopicDocument/topicdocument
        /// </summary>
        /// <returns></returns>
        //[HttpPost("http://cnhuam0webstg85:6003/api/TopicDocument")]
        public async Task<Result<string>?> CreateAsync(SearchTopicDto input) => await httpRemoteService.PostAsAsync<Result<string>>($"{BaseHostUrl}/api/TopicDocument",
            builder => builder.SetContent(input, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        /// <summary>
        /// 开放搜索接口，IT Portal的全局搜索
        /// 实际请求接口：/api/TopicDocument/topicdocument
        /// </summary>
        /// <returns></returns>
        //[HttpPut("http://cnhuam0webstg85:6003/api/TopicDocument")]
        public async Task<Result?> UpdateAsync(SearchTopicDto input) => await httpRemoteService.PutAsAsync<Result>($"{BaseHostUrl}/api/TopicDocument",
            builder => builder.SetContent(input, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));


        /// <summary>
        /// 开放搜索接口，IT Portal的全局搜索
        /// 实际请求接口：/api/TopicDocument/topicdocument
        /// </summary>
        /// <returns></returns>
        //[HttpDelete("http://cnhuam0webstg85:6003/api/TopicDocument")]
        public async Task<Result?> DeleteAsync(dynamic id) => await httpRemoteService.DeleteAsAsync<Result>($"{BaseHostUrl}/api/TopicDocument/{id}",
            builder => builder.AddAuthentication("Bearer", GetToken()));

    }
}
