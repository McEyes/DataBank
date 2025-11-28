using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.Schedule;
using Furion.TimeCrontab;

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
    [Cron("0 0/5 * * * ?", CronStringFormat.WithSeconds)]
    public class JobTemp : Furion.Schedule.IJob
    {
        private readonly ILogger<JobTemp> _logger;
        private static bool _IsRunning = false;
        public JobTemp(ILogger<JobTemp> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            var hostName = DockerUtil.GetHostName();
            if (!hostName.Equals(App.GetConfig<string>("RunSchedule"), StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogInformation($"{context} not run Auto Apply Job,HostName： {hostName}");
                return;
            }
            if (_IsRunning)
            {
                _logger.LogInformation($"{context} is running, skip");
                return;
            }
            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            finally
            {
                _IsRunning = false;
            }
            await Task.CompletedTask;
        }

    }
}
