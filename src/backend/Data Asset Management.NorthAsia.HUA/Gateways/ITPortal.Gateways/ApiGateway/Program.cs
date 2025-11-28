using ApiGateway.Extensions;
using ApiGateway.Middleware;
using ApiGateway.Models;

using Consul;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

// builder
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
      .AddJsonFile("ocelot.json") // primary config file
      .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json");

builder.Host.ConfigureAppConfiguration(AuthenticationExtensions.LoadConsulKeyValueConfig);

builder.Services.AddConsulConfig(builder.Configuration);

builder.Services
    .AddOcelot(builder.Configuration)
    .AddConsul()
    .AddConfigStoredInConsul();

// Authentication
var DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
builder.Services
    .AddAuthentication()
    .AddJwtBearer2(DefaultScheme, builder.Configuration);

// 配置CORS（跨域资源共享）
builder.Services.AddCors2(builder.Configuration);
// 配置Kestrel服务器以支持大文件上传
builder.WebHost.ConfigureKestrel(options =>
{
    // 设置最大请求体大小为100MB（100 * 1024 * 1024 = 104,857,600字节）
    options.Limits.MaxRequestBodySize = builder.Configuration.GetValue<AppSetting>("AppSetting",new AppSetting()).MaxRequestBodySize;// 104857600;
});

// 配置全局请求大小限制
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = builder.Configuration.GetValue<AppSetting>("AppSetting", new AppSetting()).MaxRequestBodySize;//104857600; // 100MB
});

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
}

builder.Services.AddHealthChecks();


// 绑定访问控制配置
var accessControlSettings = new AccessControlSettings();
builder.Configuration.GetSection("AccessControl").Bind(accessControlSettings);
builder.Services.AddSingleton(accessControlSettings);

// app
var app = builder.Build();

// 4. 处理反向代理的转发头（确保获取真实客户端IP）
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHealthChecks("/health");
// 启用CORS
app.UseCors("Default");

// 使用访问控制中间件（确保在Ocelot之前）
app.UseMiddleware<AccessControlMiddleware>();
await app.UseOcelot();
await app.UseConsul();
await app.RunAsync();
