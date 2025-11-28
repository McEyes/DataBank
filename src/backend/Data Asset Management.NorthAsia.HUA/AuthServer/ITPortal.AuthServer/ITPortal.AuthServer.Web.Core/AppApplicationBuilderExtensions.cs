using ITPortal.Core.DistributedCache;

using Furion;


using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataAssetManager.DataApiServer.Web.Core;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using Furion.SpecificationDocument;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using ITPortal.Core.Services;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace ITPortal.AuthServer.Web.Core
{
    public static class AppApplicationBuilderExtensions
    {


        //public static IWebHostBuilder UseConsulConfig(this IWebHostBuilder builder, string[] args, string configKey)
        //{
        //    IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddCommandLine(args).Build();
        //    builder.ConfigureAppConfiguration(delegate (WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        //    {
        //        hostingContext.Configuration = config.Build();
        //        IWebHostEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
        //        if (hostingContext.Configuration.GetValue<bool>("Consul:Enabled"))
        //        {
        //            string consulUrl = hostingContext.Configuration.GetValue<string>("Consul:Host");
        //            config.AddConsul(configKey, delegate (IConsulConfigurationSource options)
        //            {
        //                options.ConsulConfigurationOptions = delegate (ConsulClientConfiguration cco)
        //                {
        //                    cco.Address = new Uri(consulUrl);
        //                };
        //                options.Optional = true;
        //                options.ReloadOnChange = true;
        //                options.OnLoadException = delegate (ConsulLoadExceptionContext exceptionContext)
        //                {
        //                    exceptionContext.Ignore = true;
        //                };
        //            });
        //            hostingContext.Configuration = config.Build();
        //        }

        //        string value = hostingContext.Configuration.GetValue<string>("App:SelfUrl");
        //        builder.UseUrls(value);
        //    });
        //    return builder;
        //}


        public static async void UseConsul(this IApplicationBuilder app)
        {
            var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            using var scope = app.ApplicationServices.CreateScope();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();

            bool isEnabled = configuration.GetValue<bool>("Consul:Enabled");
            string serviceName = configuration.GetValue<string>("Consul:Service");
            var appHostUrl = configuration.GetValue<string>("Consul:ServiceUrl");
            Uri appUrl = new Uri(appHostUrl, UriKind.Absolute);
            if (!isEnabled)
                return;

            string serviceId = Guid.NewGuid().ToString();
            if (!appHostUrl.IsNullOrEmpty())
            {
                serviceId = Md5($"{appUrl.Host}:{appUrl.Port}");
            }
            string consulServiceId = $"{serviceName}:{serviceId}";

            var client = scope.ServiceProvider.GetService<IConsulClient>();

            var consulServiceRegistration = new AgentServiceRegistration
            {
                Name = serviceName,
                ID = consulServiceId,
                Address = appUrl.Host,
                Port = appUrl.Port,
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务停止多久后注销
                    Interval = TimeSpan.FromSeconds(3),//健康检查时间间隔，或者称为心跳 间隔
                    HTTP = $"http://{appUrl.Host}:{appUrl.Port}/health",//健康检查地址 
                    Timeout = TimeSpan.FromSeconds(15)   //超时时间
                }
            };

            await client.Agent.ServiceRegister(consulServiceRegistration);//.Wait();
            appLifetime.ApplicationStopping.Register(async () =>
            {
                await client.Agent.ServiceDeregister(consulServiceRegistration.ID);//.Wait();
            });
        }


        public static string HostUrl=string.Empty;
        public static void UseServerHost(this IApplicationBuilder app, IConfiguration configuration)
        {
            // 保存服务器URL
            app.Use((context, next) =>
            {
                HostUrl = $"{context.Request.Scheme}://{context.Request.Host}";
                Console.WriteLine($"Server URL: {HostUrl}");
                return next();
            });
        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="content">要加密的内容</param>
        /// <param name="isUpper">是否大写，默认小写</param>
        /// <param name="is16">是否是16位，默认32位</param>
        /// <returns></returns>
        private static string Md5(string content, bool isUpper = false, bool is16 = false)
        {
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
            string md5Str = BitConverter.ToString(result);
            md5Str = md5Str.Replace("-", "");
            md5Str = isUpper ? md5Str : md5Str.ToLower();
            return is16 ? md5Str.Substring(8, 16) : md5Str;
        }
    }

}
