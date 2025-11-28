using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.Schedule;
using Furion.TimeCrontab;

using ITPortal.Core.Emails;
using ITPortal.Core.Encrypt;
using ITPortal.Core.ProxyApi.Dto;

using ITPortal.Core.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.Jobs
{
    /// <summary>
    /// 检查生成失败的api，重新生成
    /// </summary>
    //[Cron("24,* * * *")]
    [Cron("0 20 */1 * * ?", CronStringFormat.WithSeconds)]
    public class AutoCreateApiJob : Furion.Schedule.IJob
    {
        private readonly ILogger<AutoCreateApiJob> _logger;
        private readonly IEmailSender _emailSender;
        private static bool _IsRunning = false;
        public AutoCreateApiJob(ILogger<AutoCreateApiJob> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            var hostName = DockerUtil.GetHostName();
            if (!hostName.Equals(App.GetConfig<string>("RunSchedule"), StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogInformation($"{context} not run Create Mapp Table Api Job,HostName： {hostName}");
                return;
            }
            if (_IsRunning)
            {
                _logger.LogInformation($"{context} is running, skip");
                return;
            }
            try
            {
                var apiService = context.ServiceProvider.GetService<IDataApiService>();
                var tables = await apiService.GetNoApiTables();
                var result = await apiService.CreateMappTableApi(tables);
                if (!result.Success)
                {
                    await _emailSender.SendAsync(new EmailMessage()
                    {
                        MailTo = new string[] { "Yang_Li9954@jabil.com" },
                        Subject = $"同步生成主题api服务异常",
                        Html = @$"Hi:<br>     {result.Msg}<br>异常表清单：{string.Join(",", result.Data.Select(f => f.ApiName).ToArray())}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                await _emailSender.SendAsync(new EmailMessage()
                {
                    MailTo = new string[] { "Yang_Li9954@jabil.com" },
                    Subject = $"同步生成主题api服务异常",
                    Html = @$"Hi:<br>     {ex.Message},\r\n {ex.StackTrace}"
                });
            }
            finally
            {
                _IsRunning = false;
            }
            await Task.CompletedTask;
        }

    }
}
