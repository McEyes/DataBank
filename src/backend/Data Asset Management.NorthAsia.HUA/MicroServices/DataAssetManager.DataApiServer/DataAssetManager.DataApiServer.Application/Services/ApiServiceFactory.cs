using ITPortal.Core.DataSource;
using Microsoft.Extensions.DependencyInjection;

namespace DataAssetManager.DataApiServer.Application.Services
{
    public class ApiServiceFactory
    {
        public static IApiServices CreateService(HttpContext context, ConfigType apiType)
        {
            switch (apiType)
            {
                case ConfigType.SQL:
                    return context.RequestServices.GetRequiredService<ApiSqlServiceHandler>();
                case ConfigType.FORM:
                    return context.RequestServices.GetRequiredService<ApiFormServiceHandler>();
                case ConfigType.SCRIPT:
                    break;
                case ConfigType.JSON:
                    break;
            }
            throw new NotImplementedException($"当前还不支持该API格式执行:{apiType}");
        }
    }

}
