using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.Schedule;
using Furion.TimeCrontab;

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
    /// 失败流程重新触发
    /// </summary>
    //[Cron("24,* * * *")]
    [Cron("0 0/5 * * * ?", CronStringFormat.WithSeconds)]
    public class SynchAuthApplyFlowStatus : Furion.Schedule.IJob
    {
        private readonly ILogger<SynchAuthApplyFlowStatus> _logger;
        public SynchAuthApplyFlowStatus(ILogger<SynchAuthApplyFlowStatus> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{context}");
            var authApply = context.ServiceProvider.GetService<IDataAuthApplyService>();
            var recordList = await authApply.AsQueryable().Where(f => f.Status == -2).ToListAsync();
            var successList = new List<DataApi.Dtos.DataAuthApplyEntity>();
            _logger.LogInformation($"失败流程重新发起服务：{recordList.Count}");
            foreach (var record in recordList)
            {
                var result = await authApply.StartFlow(record);
                if (result.Success)
                {
                    record.Status = -1;
                    successList.Add(record);
                }
            }
            var result2 = await authApply.BulkUpdate(successList);
            _logger.LogInformation($"重新成功发起服务：{successList.Count}，发起结果：{result2}");
            _logger.LogInformation(context.ConvertToJSON());
            await Task.CompletedTask;
        }

    }
}
