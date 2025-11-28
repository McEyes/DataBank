using ApiGateway.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Values;
using Microsoft.Extensions.Configuration;
using Consul;
using System.Net;
using Winton.Extensions.Configuration.Consul;
using System.Security.Cryptography;

namespace ApiGateway.Extensions
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddJwtBearer2(this AuthenticationBuilder builder, string authenticationScheme, ConfigurationManager configuration)
        {
            // JWT
            var jWTSettings = configuration.GetSection("JWTSettings").Get<JWTSettings>();
            
            ArgumentNullException.ThrowIfNull(jWTSettings);

            //获取安全秘钥
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jWTSettings.SecurityKey));
            //token要验证的参数集合
            var tokenValidationParameters = new TokenValidationParameters
            {
                //必须验证安全秘钥
                ValidateIssuerSigningKey = jWTSettings.ValidateIssuerSigningKey,
                //赋值安全秘钥
                IssuerSigningKey = signingKey,
                //必须验证签发人
                ValidateIssuer = jWTSettings.ValidateIssuer,
                //赋值签发人
                ValidIssuer = jWTSettings.ValidIssuer,
                //必须验证受众
                ValidateAudience = jWTSettings.ValidateAudience,
                //赋值受众
                ValidAudience = jWTSettings.ValidAudience,
                //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                ValidateLifetime = jWTSettings.ValidateLifetime,
                //允许的服务器时间偏移量
                ClockSkew = TimeSpan.FromSeconds(jWTSettings.ClockSkew),
                //是否要求Token的Claims中必须包含Expires
                RequireExpirationTime = true,
            };

            builder.AddJwtBearer(authenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            return builder;
        }


        public static async Task UseConsul(this IApplicationBuilder app)
        {
            var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            using var scope = app.ApplicationServices.CreateScope();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();
            var consulHost = configuration["GlobalConfiguration:ServiceDiscoveryProvider:Host"];
            var consulPort = 8500;
            if (int.TryParse(configuration["GlobalConfiguration:ServiceDiscoveryProvider:Port"], out int port))
                consulPort = port;
                
            var consulAddress = new Uri($"http://{consulHost}:{consulPort}");
            if (string.IsNullOrWhiteSpace(consulHost))
            {
                Console.WriteLine("Consul主机地址未配置，跳过Consul配置加载");
                return;
            }

            var serviceName = configuration["GlobalConfiguration:Service"] ?? "apigateway";
            var servicePort = configuration["GlobalConfiguration:ServicePort"] ?? "55001";
            var key = $"{serviceName}/appsettings.json";
            Console.WriteLine($"Load Consul Config Key Path:{key} ");


            var hostName = DockerUtil.GetHostName();
            var serviceUrl = $"http://{hostName}:{servicePort}";
            Console.WriteLine($"ServiceUrl {serviceUrl}...");

            Uri appUrl = new Uri(serviceUrl, UriKind.Absolute);

            string serviceId = string.Empty;
                if (!string.IsNullOrWhiteSpace(hostName))
                    serviceId = $"{hostName}:{Dns.GetHostName()}";
                else if (!string.IsNullOrWhiteSpace(serviceUrl))
                    serviceId = Md5($"{appUrl.Host}:{appUrl.Port}");
                else
                    serviceId = Guid.NewGuid().ToString();
            string consulServiceId = $"{serviceName}:{serviceId}";
            var tags = new List<string>() { $"urlPrefix-/{serviceName}" };
         
            var client = scope.ServiceProvider.GetService<IConsulClient>();
            var consulServiceRegistration = new AgentServiceRegistration
            {
                Name = serviceName,
                ID = consulServiceId,
                Address = appUrl.Host,
                Port = appUrl.Port,
                Check = new AgentServiceCheck
                {
                    Interval = TimeSpan.FromSeconds(15),//健康检查时间间隔，或者称为心跳 间隔
                    HTTP = $"{appUrl.AbsoluteUri}health",//健康检查地址 
                    Timeout = TimeSpan.FromSeconds(15),   //超时时间
                },
                Meta = new Dictionary<string, string>() { { "Weight", "1"} },
                Tags = tags.ToArray()//
            };

            app.UseHealthChecks("/health");

            try
            {
                Console.WriteLine($"Consul Register {serviceUrl}  {consulServiceId}...");
                await client.Agent.ServiceRegister(consulServiceRegistration);//.Wait();
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
                                                                                               
            // 从Ocelot配置中获取Consul地址
            var consulHost = context.Configuration["GlobalConfiguration:ServiceDiscoveryProvider:Host"];
            var consulPort = 8500;
            if (int.TryParse(context.Configuration["GlobalConfiguration:ServiceDiscoveryProvider:Port"], out int port))
                consulPort = port;
            var consulAddress = new Uri($"http://{consulHost}:{consulPort}");
            if (string.IsNullOrWhiteSpace(consulHost))
            {
                Console.WriteLine("Consul主机地址未配置，跳过Consul配置加载");
                return;
            }

            var serviceName = context.Configuration["GlobalConfiguration:Service"] ?? "apigateway";
            var key = $"{serviceName}/appsettings.json";
            Console.WriteLine($"Load Consul Config Key Path:{key} ");
            config.AddConsul(key, options =>
            {
                options.ConsulConfigurationOptions = cco => { cco.Address = consulAddress; }; // 1、consul地址
                options.Optional = true; // 2、配置选项
                options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
            });
            context.Configuration = config.Build(); // 5、consul中加载的配置信息加载到Configuration对象，然后通过Configuration 对象加载项目中
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
