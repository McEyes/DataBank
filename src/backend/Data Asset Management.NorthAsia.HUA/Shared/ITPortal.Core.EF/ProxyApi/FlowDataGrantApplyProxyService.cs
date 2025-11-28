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
    public class FlowDataGrantApplyProxyService : BaseProxyService
    {
        //private readonly IHttpRemoteService httpRemoteService;
        private readonly string BaseHostUrl;
        public FlowDataGrantApplyProxyService(IHttpRemoteService httpRemoteService):base(httpRemoteService) 
        {
            //this.httpRemoteService = httpRemoteService;
            BaseHostUrl = App.GetConfig<string>("RemoteApi:BaseUrl");
        }

        public async Task<Result?> InitFlowAsync(FlowDataGrantInfo input) => await httpRemoteService.PostAsAsync<Result>($"{BaseHostUrl}/homeapi/api/home/FlowDataGrantApply/InitFlow",
            builder => builder.SetContent(input, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        public async Task<Result?> DeleteAsync(string applyFormNo) => await httpRemoteService.PostAsAsync<Result>($"{BaseHostUrl}/homeapi/api/home/FlowDataGrantApply/DeleteAsync?id={applyFormNo}",
            builder => builder.SetContent(new object(), "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

    }
}
