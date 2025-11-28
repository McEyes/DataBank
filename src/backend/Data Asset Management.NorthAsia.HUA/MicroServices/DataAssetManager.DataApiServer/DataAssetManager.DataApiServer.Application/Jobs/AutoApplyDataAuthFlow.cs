using DataAssetManager.DataTableServer.Application;

using Furion.Schedule;
using Furion.TimeCrontab;

using ITPortal.Core.Encrypt;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Flow.Enums;
using ITPortal.Core.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.Jobs
{
    /// <summary>
    /// 自动审批流程，状态没有完成的自动审批，每分钟检查一次
    /// </summary>
    //[Cron("1,* * * *")]
    [Cron("0 0/1 * * * ?", CronStringFormat.WithSeconds)]
    public class AutoApplyDataAuthFlow : Furion.Schedule.IJob
    {
        private readonly ILogger<AutoApplyDataAuthFlow> _logger;
        private static bool _IsRunning =false;
        public AutoApplyDataAuthFlow(ILogger<AutoApplyDataAuthFlow> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            try
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
                _IsRunning = true;
                _logger.LogInformation($"{context}");
                var authApply = context.ServiceProvider.GetService<IDataAuthApplyService>();
                var flowService = context.ServiceProvider.GetService<FlowApplyProxyService>();
                var isPublicLevel = new string[] { "1", "2" };
                var recordList = await authApply.AsQueryable().Where(f => f.Status == -1 && isPublicLevel.Contains(f.LevelId)).Distinct().ToListAsync();
                var successList = new List<DataApi.Dtos.DataAuthApplyEntity>();
                _logger.LogInformation($"自动审批流程失败重新自动审批服务：{recordList.Count}");
                var token = JwtHelper.GenerateJwtToken(new UserInfo()
                {
                    ChineseName = "auto approve",
                    EnglishName = "auto approve",
                    Department = "auto approve",
                    Email = "Anny_Wu@jabil.com",
                    Id = "AutoApproveUser",
                    Name = "Auto Approve User",
                    NtId = "AutoApproveUser",
                    Surname = "AutoApproveUser",
                    UserId = "AutoApproveUser",
                    UserName = "AutoApproveUser",
                    PhoneNumber = "",
                    Roles = new List<string>()
                });

                StringBuilder errmsg = new StringBuilder();
                int successCount = 0, failCount = 0;
                foreach (var record in recordList)
                {
                    var result = await flowService.SendApproveAsync(new ITPortal.Core.ProxyApi.Flow.Dto.FlowAuditDto()
                    {
                        FlowInstId = record.Id,
                        ActOperate = FlowAction.Approval.ToString(),
                        AuditContent = "公共级别自动审批",
                    }, token);
                    if (!result.Success)
                    {
                        failCount++;
                        errmsg.AppendLine($"发起流程[{record.AppName}][{record.Id}]结果：[{result.Code}]{result.Msg}");
                    }
                    else
                    {
                        successCount++;
                    }
                }
                _logger.LogInformation($"自动审批流程失败重新自动审批服务,成功：{successCount}，失败:{failCount}，自动审批结果：{errmsg}");
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
