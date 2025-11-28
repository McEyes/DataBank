using Elastic.Clients.Elasticsearch;

using Furion.Schedule;
using Furion.TimeCrontab;

using ITPortal.AuthServer.Application.EmployeeInfos.Dtos;
using ITPortal.AuthServer.Application.EmployeeInfos.Services;
using ITPortal.Core.Emails;
using ITPortal.Core.Encrypt;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;
using Pipelines.Sockets.Unofficial.Buffers;
using static Grpc.Core.Metadata;

using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace ITPortal.AuthServer.Application.Jobs
{
    /// <summary>
    /// 同步员工信息,一小时同步一次
    /// 每天凌晨4点同步
    /// </summary>
#if DEBUG
    [Cron("0/10 * * * * ?", CronStringFormat.WithSeconds)]
#else
    [Cron("0 0 4 */1 * ?", CronStringFormat.WithSeconds)]
#endif
    public class SynEmployeeInfoJob : Furion.Schedule.IJob
    {
        private readonly ILogger<SynEmployeeInfoJob> _logger;
        private static bool _IsRunning = false;
        private readonly EmployeeProxyService _employeeProxyService;
        private readonly IEmployeeBaseInfoService _employeeBaseInfoService;
        private readonly IEmailSender _emailSender;
        public SynEmployeeInfoJob(ILogger<SynEmployeeInfoJob> logger, EmployeeProxyService employeeProxyService, IEmployeeBaseInfoService employeeBaseInfoService, IEmailSender emailSender)
        {
            _logger = logger;
            _employeeProxyService = employeeProxyService;
            _employeeBaseInfoService = employeeBaseInfoService;
            _emailSender = emailSender;
        }

        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            try
            {
                var hostName = DockerUtil.GetHostName();
                if (!hostName.Equals(App.GetConfig<string>("RunSchedule"), StringComparison.CurrentCultureIgnoreCase))
                {
                    _logger.LogInformation($"{context} not run Syn EmployeeInfo Job,HostName： {hostName}");
                    return;
                }
                if (_IsRunning)
                {
                    _logger.LogInformation($"{context} is running Syn EmployeeInfo Job, skip");
                    return;
                }
                _logger.LogInformation($"{context} running Syn EmployeeInfo Job");
                _IsRunning = true;
                var result = await _employeeProxyService.AllMasterEmployeeAsync();
                if (result?.Success == true)
                {
                    var dataResult = result as Result<PageResult<MasterEmployeeInfo>>;
                    var list = dataResult.Data.Data.Adapt<List<EmployeeBaseInfo>>();
                    await _employeeBaseInfoService.ImportData(list);
                }
                else
                {
                    _logger.LogError($"{context} is running Syn EmployeeInfo Job Errir:{result?.Msg}");
                    await _emailSender.SendAsync(new EmailMessage()
                    {
                        MailTo = new string[] { "Yang_Li9954@jabil.com" },
                        Subject = $"同步EmployeeBaseInfo服务获取信息异常",
                        Html = @$"Hi:<br>     {result.Msg}"
                    });
                }
            }
            catch (Exception ex)
            {
                await _emailSender.SendAsync(new EmailMessage()
                {
                    MailTo = new string[] { "Yang_Li9954@jabil.com" },
                    Subject = $"同步EmployeeBaseInfo服务异常",
                    Html = @$"Hi:<br>     {ex.Message},\r\n {ex.StackTrace}"
                });
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
