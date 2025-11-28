using Consul;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using System.Security.Cryptography;
using System.Text;
using Furion;
using ITPortal.Extension.System;
using System.Collections.Generic;
using ITPortal.Core.Encrypt;
using Winton.Extensions.Configuration.Consul;
using System.Net;

namespace ITPortal.Core.Extensions
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
            var consulOptions = configuration.Get<ConsulOptions>("Consul");
            if (!consulOptions.Enabled)
                return;


            var hostName = DockerUtil.GetHostName();
            if (consulOptions.ServiceUrl.IsNullOrWhiteSpace())
            {
                if (consulOptions.ServiceHost.IsNullOrWhiteSpace())
                {
                    consulOptions.ServiceUrl = $"http://{hostName}:{consulOptions.Port}";
                    Console.WriteLine($"hostName： {consulOptions.ServiceUrl}...");
                }
                else
                {
                    consulOptions.ServiceUrl = $"https://{consulOptions.ServiceHost}:{consulOptions.Port}";
                    Console.WriteLine($"Host： {consulOptions.ServiceUrl}...");
                }
            }
            Console.WriteLine($"ServiceUrl {consulOptions.ServiceUrl}...");

            Uri appUrl = new Uri(consulOptions.ServiceUrl, UriKind.Absolute);

            string serviceId = consulOptions.Id;
            string consulServiceId = consulOptions.Id;
            if (serviceId.IsNullOrWhiteSpace())
            {
                if (hostName.IsNotNullOrWhiteSpace())
                    serviceId = $"{hostName}:{Dns.GetHostName()}";
                else if (consulOptions.ServiceUrl.IsNotNullOrWhiteSpace())
                    serviceId = Md5($"{appUrl.Host}:{appUrl.Port}");
                else
                    serviceId = Guid.NewGuid().ToString();
                consulServiceId = $"{consulOptions.Service}:{serviceId}";
            }
            var tags = new List<string>() { $"urlPrefix-/{consulOptions.Service}" };
            if (consulOptions.Tags.IsNotNullOrWhiteSpace())
                tags.AddRange(consulOptions.Tags.Split(new char[] { ',', ';', '、' }, StringSplitOptions.RemoveEmptyEntries));

            var client = scope.ServiceProvider.GetService<IConsulClient>();
            var consulServiceRegistration = new AgentServiceRegistration
            {
                Name = consulOptions.Service,
                ID = consulServiceId,
                Address = appUrl.Host,
                Port = appUrl.Port,
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = consulOptions.DeregisterCriticalServiceAfter > 0 ? TimeSpan.FromSeconds(consulOptions.DeregisterCriticalServiceAfter) : null,//服务停止多久后注销
                    Interval = TimeSpan.FromSeconds(consulOptions.Interval),//健康检查时间间隔，或者称为心跳 间隔
                    HTTP = $"{appUrl.AbsoluteUri}{consulOptions.healthUrl}",//健康检查地址 
                    Timeout = TimeSpan.FromSeconds(consulOptions.Timeout),   //超时时间
                },
                Meta = new Dictionary<string, string>() { { "Weight", consulOptions.Weight.ToString() } },
                Tags = tags.ToArray()//
            };

            app.UseHealthChecks("/" + consulOptions.healthUrl);
            //app.UseEndpoints(endpoints =>
            //{
            //    //设置健康检查终结点
            //    endpoints.MapHealthChecks($"/{consulOptions.healthUrl}");
            //});
            try
            {
                Console.WriteLine($"Consul Register {consulOptions.ServiceUrl}  {consulServiceId}...");
                await client.Agent.ServiceRegister(consulServiceRegistration);//.Wait();
                if (consulOptions.DeregisterCriticalServiceAfter > 0)
                {
                    appLifetime.ApplicationStopping.Register(() =>
                    {
                        Console.WriteLine("应用程序正在停止...");
                        client.Agent.ServiceDeregister(consulServiceRegistration.ID).Wait();
                    });
                }
            }
            catch (ConsulRequestException ex)
            {
                Console.WriteLine($"服务注册失败: {ex.Message}");
                if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("可能是ACL令牌无效或权限不足");
                }
            }
        }

        public static void LoadConsulKeyValueConfig(HostBuilderContext context, IConfigurationBuilder config)
        {
            // 加载默认配置信息到Configuration
            context.Configuration = config.Build();
            var consulOptions = context.Configuration.Get<ConsulOptions>("Consul");
            if (!consulOptions.Enabled) return;
            //动态获取项目名称
            //var evn = context.HostingEnvironment;
            //var hostName = DockerUtil.GetHostName();{hostName}/
            var key = $"{consulOptions.Service}/appsettings.json";
            Console.WriteLine($"Load Consul Config Key Path:{key} ");
            config.AddConsul(key, options =>
            {
                options.ConsulConfigurationOptions = cco =>
                {
                    cco.Address = new Uri(consulOptions.Host);
                    cco.Token = consulOptions.Token;  // 关键：携带令牌访问Consul
                }; // 1、consul地址
                options.Optional = true; // 2、配置选项
                options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
            });
            context.Configuration = config.Build(); // 5、consul中加载的配置信息加载到Configuration对象，然后通过Configuration 对象加载项目中
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
