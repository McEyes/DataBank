using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;

namespace DataAssetManager.DataApiServer.Web.Core
{
    /// <summary>
    /// 动态路由数据源，项目启动的时候创建路由规则，项目启动后添加的路由规则不生效
    /// </summary>
    public class DynamicEndpointDataSource : EndpointDataSource
    {
        public DynamicEndpointDataSource() {
            var routePattern = RoutePatternFactory.Parse("text");
            var endpoint = new RouteEndpoint(
                async context => await DefaultRouteRedirectAction(context),
                routePattern,
                order: 0,
                new EndpointMetadataCollection(new HttpMethodMetadata(new List<string>() { "GET" })),//
                displayName: $"Dynamic:text"
            );
            _endpoints.Add(endpoint);
        }

        private async Task DefaultRouteRedirectAction(HttpContext context)
        {
            //调用真是路由
            Console.WriteLine("DefaultRouteRedirectAction");
            await Task.FromResult(context);
        }


        private readonly List<Endpoint> _endpoints = new();
        private CancellationTokenSource _cts = new();
        public override IReadOnlyList<Endpoint> Endpoints => _endpoints;
        public override IChangeToken GetChangeToken() => new CancellationChangeToken(_cts.Token);

        public void AddEndpoint(Endpoint endpoint)
        {
            try
            {
                _endpoints.Add(endpoint);
                _cts.TryReset();
                //_cts.Cancel();
                //_cts.Dispose();
                //_cts = new CancellationTokenSource();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
