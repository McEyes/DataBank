
using Furion.Schedule;
using Furion.TimeCrontab;

using ITPortal.Core.Emails;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Flow.Enums;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Flow.Application.EmailSendRecord;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Flow.Application.Jobs
{
    /// <summary>
    /// 发送待办邮件服务
    /// </summary>
    //[Cron("1,* * * *")]
    [Cron("0 0/1 * * * ?", CronStringFormat.WithSeconds)]
    public class SendTodoEmailJob : Furion.Schedule.IJob
    {
        private readonly ILogger<SendTodoEmailJob> _logger;
        private static bool _isRunning = false;
        public SendTodoEmailJob(ILogger<SendTodoEmailJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            try
            {
                var hostName = ITPortal.Core.Encrypt.DockerUtil.GetHostName();
                if (!hostName.Equals(App.GetConfig<string>("RunSchedule"), StringComparison.CurrentCultureIgnoreCase))
                {
                    _logger.LogInformation($"{context} not run Send Todo Email Job Job,HostName： {hostName}");
                    return;
                }
                _logger.LogInformation($"{context}");
                if (_isRunning)
                {
                    _logger.LogInformation($"{context} The last round of services is running");
                    return;
                }
                _isRunning = true;
                _logger.LogInformation($"{context}");
                var emailRecordService = context.ServiceProvider.GetService<IEmailSendRecordService>();
                var emailSender = context.ServiceProvider.GetService<IEmailSender>();
                var needSendStatus = new int[] { 0, -1 };
                var recordList = await emailRecordService.AsQueryable().Where(f => needSendStatus.Contains(f.Status.Value) && f.RetryTimes >= 0).OrderBy(f=>f.CreateTime).ToListAsync();
                _logger.LogInformation($"Send pending emails：{recordList.Count}");
                StringBuilder errmsg = new StringBuilder();
                foreach (var record in recordList)
                {
                    try
                    {
                        await emailSender.SendAsync(new EmailMessage()
                        {
                            MailTo = SplitEmail(record.EmailTo),
                            MailCC = SplitEmail(record.EmailCc),
                            MailBCC = SplitEmail(record.EmailBcc),
                            Subject = record.EmailSubject,
                            Html = record.EmailHtmlBody
                        });
                        record.Status = 1;
                        record.SendTime = DateTimeOffset.Now;
                    }
                    catch (Exception ex)
                    {
                        record.RetryTimes--;
                        record.Status = -1;
                        record.ErrorMsg = ex.Message;
                        _logger.LogError($"{record.EmailSubject} email send error:{ex.Message}", ex);
                    }
                    await emailRecordService.Modify(record);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" email send error:{ex.Message}", ex);
            }
            finally { _isRunning = false; }
            await Task.CompletedTask;
        }

        private string[] SplitEmail(string emailStr)
        {
            if (emailStr.IsNullOrWhiteSpace()) return new string[0];
            return emailStr.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
