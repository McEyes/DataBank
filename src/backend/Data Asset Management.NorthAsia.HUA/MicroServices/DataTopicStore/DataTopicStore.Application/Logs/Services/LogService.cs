using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace DataTopicStore.Application.Logs.Services
{
    public class LogService<T> : ILogService<T>, ITransient
    {
        private readonly Repository<LogsEntity> logRepository;
        public LogService(Repository<LogsEntity> logRepository)
        {
            this.logRepository = logRepository;
        }

        public async Task LogAsync(string message, string action = null, LogLevel logLevel = LogLevel.Information)
        {
            var currentUser = App.HttpContext.GetCurrUserInfo();
            var typeName = typeof(T).FullName;
            await logRepository.InsertReturnSnowflakeIdAsync(new LogsEntity
            {
                action = action,
                created_by = currentUser?.Id,
                created_time = DateTime.Now,
                loglevel = logLevel.ToString(),
                message = message,
                type_name = typeName,
                updated_by = currentUser?.Id,
                updated_time = DateTime.Now
            });
        }

        public async Task LogDebugAsync(string message, string action = null)
        {
            await LogAsync(message, action, LogLevel.Debug);
        }

        public async Task LogErrorAsync(string message, string action = null)
        {
            await LogAsync(message, action, LogLevel.Error);
        }

        public async Task LogInformationAsync(string message, string action = null)
        {
            await LogAsync(message, action, LogLevel.Information);
        }

        public async Task LogTraceAsync(string message, string action = null)
        {
            await LogAsync(message, action, LogLevel.Trace);
        }

        public async Task LogWarningAsync(string message, string action = null)
        {
            await LogAsync(message, action, LogLevel.Warning);
        }
    }
}
