
using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using MySqlX.XDevAPI.Relational;

using StackExchange.Profiling.Internal;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAssetManager.DataApiServer.Web.Core
{
    public class AuthorizeMiddleware
    {
        private readonly IDistributedCacheService _cache;
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthorizeMiddleware> _logger;
        private readonly IDataAuthAuditRecordService _dataAuthAuditRecordService;
        private readonly IDataTableService _dataTableService;
        private readonly IDataApiLogService _apiLogService;

        public AuthorizeMiddleware(RequestDelegate next, IDistributedCacheService cache
            , IDataAuthAuditRecordService dataAuthAuditRecordService
            , IDataTableService tableService,
            IDataApiLogService apiLogService
            , ILogger<AuthorizeMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
            _dataAuthAuditRecordService = dataAuthAuditRecordService;
            _dataTableService = tableService;
            _apiLogService = apiLogService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            var path = context.Request.Path.Value ?? "";
            try
            {
                if (!path.StartsWith("/services/", StringComparison.CurrentCultureIgnoreCase))
                {
                    await _next(context);
                    return;
                }
                path = path.ToLower();
                var routInfo = _cache.HashGet<RouteInfo>(DataAssetManagerConst.RouteRedisKey, path);
                if (routInfo != null && !(await CheckAuthorzie(context, routInfo) || VerifyApiTablePermission(context, routInfo.ExecuteConfig.tableId)))
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsJsonAsync(new ITPortal.Core.Services.Result()
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Msg = $"Data is not public, before do access such Data, put apiToken in request header must be essential, if not have apiToken please contact DataOwner and apply it first!!  API{routInfo.Id}"
                    });
                    //记录api调用情况
                    sw.Stop();
                    var _ = _apiLogService.SendLogEvent(context, routInfo, (int)sw.ElapsedMilliseconds, $"API{routInfo.Id}, HTTP status code 401.");
                    return;
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                //记录api调用情况
                var _ = _apiLogService.SendLogEvent(context, new RouteInfo() { ApiUrl = path }, (int)sw.ElapsedMilliseconds, $"API {path}, HTTP status code 401.\r\n ERRPR: {ex.Message}");
            }
            finally
            {
                sw.Stop();
                //_logger.LogInformation($"收到{path}请求,验证权限耗时：{sw.ElapsedMilliseconds}毫秒");
            }
            await _next(context);
        }


        private async Task<bool> VerifyApiToken(HttpContext context, RouteInfo apiInfo)
        {
            var request = context.Request;
            string token = request.Headers["x-token"];
            try
            {
                //检查x-token的复杂都，不容许直接ntid调用,20250730 后启用校验
                if (token.IsNullOrWhiteSpace() || token.Length < 10)
                {
                    _logger.LogWarning($"Api:{apiInfo.Id}'s 非法token,{apiInfo.ApiUrl}，token:{token}");
                    return false;
                }
                var tableInfo = _cache.HashGet<DataTableInfo>(DataAssetManagerConst.DataTable_HashKey, apiInfo.ExecuteConfig.tableId);
                if (tableInfo == null)
                {
                    _logger.LogWarning($"Api:{apiInfo.Id}'s 没有table信息，不容许访问,{apiInfo.ApiUrl}");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning($"Api:{apiInfo.Id}'s 没有token信息，不容许访问,{apiInfo.ApiUrl}");
                    return false;
                }
                if (_dataTableService.CheckUserHasTable(tableInfo.Id, token))
                {
                    //获取userid 
                    var clientInfo = await _apiLogService.AsQueryable<AssetClientSecretsEntity, Guid>().FirstAsync(f => f.Secrets == token);
                    var userId = token;
                    if (clientInfo != null) userId = clientInfo.ClientId;
                    var userInfo = new UserInfo()
                    {
                        Id = userId,
                        NtId = userId,
                        UserName = userId,
                        UserId = userId
                    };
                    context.Items[DataAssetManagerConst.HttpContext_UserInfo] = userInfo;
                    return true;
                }
                //if (tableInfo.IsPublicSecurityLevel)
                //{//所有表都需要申请后才能访问
                //    _logger.LogWarning($"Api:{apiInfo.Id}'s SecretKey Is Public Security Level，公共表，容许所有人直接访问,{apiInfo.ApiUrl}");
                //    return true;
                //}
                //if (string.IsNullOrWhiteSpace(token))
                //{
                //    _logger.LogWarning($"Api:{apiInfo.Id}'s 没有token信息，不容许访问,{apiInfo.ApiUrl}");
                //    return false;
                //}
                var record = (await _dataAuthAuditRecordService.Query(new DataAuthAuditRecordDto() { TableId = tableInfo.Id, ApiToken = token })).FirstOrDefault();
                if (record == null)
                {
                    _logger.LogWarning($"Api:{apiInfo.Id}'s 有token，没有table审批记录信息，非公共表，没权限,{apiInfo.ApiUrl}");
                    return false;
                }
                if (record.UserId.IsNullOrWhiteSpace())
                {
                    var userInfo = new UserInfo()
                    {
                        UserId = record.UserId
                    };
                    context.Items[DataAssetManagerConst.HttpContext_UserInfo] = userInfo;
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Api:{apiInfo.Id}'s 有token，有table审批记录信息，但是userid信息异常，非guid数据，没权限,{apiInfo.ApiUrl}");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Api:{apiInfo.Id}'s SecretKey encode error:{e.Message},\r\n{e.StackTrace}\r\n\n");
            }
            return false;
        }

        private bool VerifyApiTablePermission(HttpContext context, string tableId)
        {
            if (tableId.IsNullOrWhiteSpace()) return false;
            var userInfo = context.GetCurrUserInfo();
            if (userInfo == null || userInfo.UserId.IsNullOrWhiteSpace())
            {
                return false;
            }
            return _dataTableService.CheckUserHasTable(tableId, userInfo.UserId);
        }


        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="apiInfo"></param>
        /// <returns></returns>
        private async Task<bool> CheckAuthorzie(HttpContext httpContext, RouteInfo apiInfo)
        {

            var NoAuthorzie = await _cache.GetStringAsync($"{DataAssetManagerConst.RedisKey}NoAuthorzie");
            if (NoAuthorzie == "yes") return true;

            if (await VerifyApiToken(httpContext, apiInfo)) return true;
            // 验证是否有apikey
            try
            {
                var apikey = httpContext.Request.Headers["apikey"].ToString();//api id
                var secretkey = httpContext.Request.Headers["secretkey"].ToString();// userid 
                var apiId = ITPortal.Core.DESEncryption.Decrypt(apikey);
                var userInfo = new UserInfo();
                userInfo.UserId = ITPortal.Core.DESEncryption.Decrypt(secretkey);
                if (!string.IsNullOrWhiteSpace(userInfo.UserId))
                {
                    if (!httpContext.Items.ContainsKey(DataAssetManagerConst.HttpContext_UserInfo))
                        httpContext.Items.Add(DataAssetManagerConst.HttpContext_UserInfo, userInfo);
                    else httpContext.Items[DataAssetManagerConst.HttpContext_UserInfo] = userInfo;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" SecretKey encode error:{ex.Message},\r\n{ex.StackTrace}\r\n\n");
            }
            var result = await httpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                if (!httpContext.Items.ContainsKey(DataAssetManagerConst.HttpContext_UserInfo))
                    httpContext.Items.Add(DataAssetManagerConst.HttpContext_UserInfo, new UserInfo(httpContext.User));
                else httpContext.Items[DataAssetManagerConst.HttpContext_UserInfo] = new UserInfo(httpContext.User);
            }
            return false;
        }


 
        /// <summary>
        /// 获取 JWT Bearer Token
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="headerKey"></param>
        /// <param name="tokenPrefix"></param>
        /// <returns></returns>
        public static string GetJwtBearerToken(HttpContext httpContext, string headerKey = "Authorization", string tokenPrefix = "Bearer ")
        {
            // 判断请求报文头中是否有 "Authorization" 报文头
            var bearerToken = httpContext.Request.Headers[headerKey].ToString();
            if (string.IsNullOrWhiteSpace(bearerToken)) return default;

            var prefixLenght = tokenPrefix.Length;
            return bearerToken.StartsWith(tokenPrefix, true, null) && bearerToken.Length > prefixLenght ? bearerToken[prefixLenght..].Trim() : default;
        }



        /// <summary>
        /// 读取 Token，不含验证
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static JwtSecurityToken SecurityReadJwtToken(string accessToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(accessToken);
            return jwtSecurityToken;
        }


        /// <summary>
        /// 验证 Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<(bool IsValid, JsonWebToken Token, TokenValidationResult validationResult)> Validate(string accessToken)
        {
            var jwtSettings = Furion.DataEncryption.JWTEncryption.GetJWTSettings();
            if (jwtSettings == null) return (false, default, default);
            // 加密Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey));
            var creds = new SigningCredentials(key, jwtSettings.Algorithm);
            // 创建Token验证参数
            var tokenValidationParameters = Furion.DataEncryption.JWTEncryption.CreateTokenValidationParameters(jwtSettings);
            tokenValidationParameters.IssuerSigningKey ??= creds.Key;

            // 验证 Token
            var tokenHandler = new JsonWebTokenHandler();
            try
            {
                var tokenValidationResult = await tokenHandler.ValidateTokenAsync(accessToken, tokenValidationParameters);
                if (!tokenValidationResult.IsValid) return (false, null, tokenValidationResult);

                var jsonWebToken = tokenValidationResult.SecurityToken as JsonWebToken;
                return (true, jsonWebToken, tokenValidationResult);
            }
            catch
            {
                return (false, default, default);
            }
        }

        public async Task<Dictionary<string, object>> GetAllParams(HttpContext context)
        {
            var request = context.Request;
            var paramsData = await ReadRequestBody(context);
            //ToDynamic(headers: request.Headers, paramsData);
            paramsData = ToDynamic(request.Query, paramsData);
            if (request.HasFormContentType) paramsData = ToDynamic(request.Form, paramsData);
            CheckPageParams(paramsData);
            return paramsData;
        }


        public async Task<Dictionary<string, object>> ReadRequestBody(HttpContext context)
        {
            return await ReadRequestBody<Dictionary<string, object>>(context);
        }

        public async Task<T> ReadRequestBody<T>(HttpContext context)
        {
            // 确保请求体流可重复读取
            context.Request.EnableBuffering();
            if (!context.Request.ContentLength.HasValue || context.Request.ContentLength <= 0) return default;
            // 读取请求体
            //var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            //await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            //var requestBody = Encoding.UTF8.GetString(buffer);
            var requestBody = string.Empty;
            // 初始化内存流用于累积数据
            using (var ms = new MemoryStream())
            {
                // 定义每次读取的缓冲区（大小可根据实际场景调整，例如4096字节）
                byte[] buffer = new byte[4096];
                int bytesRead;

                // 循环读取，直到流结束（bytesRead为0）
                while ((bytesRead = await context.Request.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    // 将读取到的字节写入内存流
                    await ms.WriteAsync(buffer, 0, bytesRead);
                }

                // 将内存流中的数据转换为字符串（使用UTF8编码）
                requestBody = Encoding.UTF8.GetString(ms.ToArray());
            }
            // 输出请求体数据
            //System.Console.WriteLine($"Request Body: {requestBody}");
            requestBody = requestBody.Trim('\0');
            // 将流指针重置到开头，以便后续中间件或控制器可以再次读取请求体
            context.Request.Body.Position = 0;
            return Furion.JsonSerialization.JSON.Deserialize<T>(requestBody);
        }
        public Dictionary<string, object> ToDynamic(IFormCollection from, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (from == null) return expando;

            foreach (var kvp in from)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }
        public Dictionary<string, object> ToDynamic(IQueryCollection query, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (query == null) return expando;

            foreach (var kvp in query)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        public void CheckPageParams(Dictionary<string, object> expandoDict)
        {
            if (!expandoDict.ContainsKey("pageNum")) expandoDict.Add("pageNum", 1);
            if (!expandoDict.ContainsKey("pageSize"))
            {
                if (!expandoDict.ContainsKey("maxSize"))
                    expandoDict.Add("pageSize", value: 5000);
                else
                    expandoDict.Add("pageSize", expandoDict["maxSize"]);
            }
            if (!expandoDict.ContainsKey("total")) expandoDict.Add("total", 0);
        }
    }
}