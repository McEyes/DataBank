using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using DataTopicStore.Application.DocumentFilters;
using DataTopicStore.Core;
using Furion;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace DataTopicStore.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConsoleFormatter();
        services.AddJwt<JwtHandler>();

        services.AddCorsAccessor();
       
        services.AddControllers()
                .AddInjectWithUnifyResult(c=>c.ConfigureSwaggerGen(options=> 
                {
                    options.DocumentFilter<TopicDocumentFilter>();

                }))
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new LongToStringConverter());

                });

        services.AddSqlsugarSetup();
        services.AddHealthChecks();
        services.AddTransient<IExporter, ExcelExporter>();
        services.AddTransient<IExcelExporter, ExcelExporter>();


        services.AddHttpRemote(options =>
        {
        });



    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.EnableBuffering();
        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseHealthChecks("/health");

    }
}
