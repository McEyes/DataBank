using Furion.EventBus;

using ITPortal.Core.LightElasticSearch;

using Microsoft.AspNetCore.Http;

namespace DataAssetManager.DataApiServer.Web.Core
{
    public class TraceApiLogMiddleware
    {
        private readonly RequestDelegate _next;
        protected readonly IEventPublisher EventPublisher;

        public TraceApiLogMiddleware(RequestDelegate next, IEventPublisher eventPublisher)
        {
            _next = next;
            EventPublisher = eventPublisher;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Items.Add("ApiTrackLog", new ApiTrackLogInfo()
            {
                Id = Guid.NewGuid(),
                Path = context.Request.Path,
                Method = context.Request.Method,
            });

            await _next(context);
        }
    }
}