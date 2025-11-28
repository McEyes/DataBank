using Furion.FriendlyException;
using Furion.UnifyResult;
using Furion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Furion.DataValidation;
using ITPortal.Core.Services;
using Furion.DependencyInjection;

namespace ITPortal.Search.Web.Core.Extensions
{
    [SuppressSniffer, UnifyModel(typeof(Result<>))]
    public class ITPortalResultProvider : IUnifyResultProvider
    {

        /// <summary>
        /// JWT 授权异常返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IActionResult OnAuthorizeException(DefaultHttpContext context, ExceptionMetadata metadata)
        {
            return new JsonResult(RESTfulResult(metadata.StatusCode, data: metadata.Data, errors: metadata.Errors)
                , UnifyContext.GetSerializerSettings(context));
        }

        /// <summary>
        /// 异常返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
        {
            return new JsonResult(RESTfulResult(metadata?.StatusCode ?? 503, data: metadata?.Data ?? context.Exception.Message, errors: metadata?.Errors ?? context.Exception.StackTrace)
                , UnifyContext.GetSerializerSettings(context));
        }

        /// <summary>
        /// 成功返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult OnSucceeded(ActionExecutedContext context, object data)
        {
            return new JsonResult(RESTfulResult(StatusCodes.Status200OK, true, data)
                , UnifyContext.GetSerializerSettings(context));
        }


        /// <summary>
        /// 成功返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult OnValidateFailed(ActionExecutedContext context, object data)
        {
            return new JsonResult(RESTfulResult(StatusCodes.Status400BadRequest, false, data)
                , UnifyContext.GetSerializerSettings(context));
        }

        /// <summary>
        /// 验证失败/业务异常返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
        {
            return new JsonResult(RESTfulResult(metadata.StatusCode ?? StatusCodes.Status400BadRequest
                , data: metadata.Data
                , errors: !metadata.SingleValidationErrorDisplay ? metadata.ValidationResult : metadata.FirstErrorMessage)
                , UnifyContext.GetSerializerSettings(context));
        }

        /// <summary>
        /// 特定状态码返回值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="unifyResultSettings"></param>
        /// <returns></returns>
        public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings)
        {
            // 设置响应状态码
            UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);

            switch (statusCode)
            {
                // 处理 401 状态码
                case StatusCodes.Status401Unauthorized:
                    await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, errors: "401 Unauthorized")
                        , App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                    break;
                // 处理 403 状态码
                case StatusCodes.Status403Forbidden:
                    await context.Response.WriteAsJsonAsync(RESTfulResult(statusCode, errors: "403 Forbidden")
                        , App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                    break;

                default: break;
            }
        }

        /// <summary>
        /// 返回 RESTful 风格结果集
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="succeeded"></param>
        /// <param name="data"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static Result<object> RESTfulResult(int statusCode, bool succeeded = default, object data = default, object errors = default)
        {
            return new Result<object>
            {
                Code = statusCode,
                Success = succeeded,
                Data = data,
                Msg = errors ?? (succeeded ? "success" : "failure"),
                Extras = UnifyContext.Take(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }
    }
}
