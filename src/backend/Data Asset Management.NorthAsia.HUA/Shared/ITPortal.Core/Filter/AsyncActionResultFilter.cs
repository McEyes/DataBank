using Furion.UnifyResult;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DataAssetManager.DataApiServer.Web.Core
{
    public class AsyncActionResultFilter : IAsyncResultFilter
    {
        private readonly IUnifyResultProvider _resultProvider;
        public AsyncActionResultFilter(IUnifyResultProvider resultProvider)
        {
            _resultProvider = resultProvider;
        }


        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //if (context.Exception != null)
            //{
            //    context.ExceptionHandled = true;
            //    context.Result = _resultProvider.OnException(new ExceptionContext(context, context.Filters), null);
            //}
            //else if (context.Result is ObjectResult)
            //{
            //    context.Result = _resultProvider.OnSucceeded(context, (context.Result as ObjectResult).Value);
            //}
            return Task.CompletedTask;
        }
    }
}