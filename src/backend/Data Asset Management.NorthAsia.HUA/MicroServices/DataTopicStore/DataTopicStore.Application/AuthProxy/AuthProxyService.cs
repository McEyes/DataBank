using DataTopicStore.Application.AuthProxy.Dtos;
using DataTopicStore.Application.Common.Dtos;
using Furion.HttpRemote;

namespace DataTopicStore.Application.UserProxy
{
    public class AuthProxyService : ITransient
    {
        private readonly IHttpRemoteService httpRemoteService;
        private readonly string BaseHostUrl;
        private string Authorization = "";
        public AuthProxyService(IHttpRemoteService httpRemoteService)
        {
            this.httpRemoteService = httpRemoteService;
            BaseHostUrl = App.GetConfig<string>("RemoteApi:AuthHostUrl");
        }

        private string GetToken()
        {
            var token = string.Empty;
            if (App.HttpContext != null && App.HttpContext.Request.Headers != null)
                token = App.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(token)) token = token.Replace("Bearer ", "");
            return token;
        }

        public async Task<HttpResult<EmployeeInfoDto>?> GetEmployeeAsync(string WorkNTID) => await httpRemoteService.GetAsAsync<HttpResult<EmployeeInfoDto>>($"{BaseHostUrl}/api/Employee/info/{WorkNTID}",
            builder => builder.AddAuthentication("Bearer", GetToken()));

    }
}
