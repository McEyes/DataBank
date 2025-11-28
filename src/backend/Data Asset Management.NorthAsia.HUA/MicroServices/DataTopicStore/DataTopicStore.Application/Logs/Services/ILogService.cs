using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DataTopicStore.Application.Logs.Services
{
    public interface ILogService<T>
    {
        Task LogAsync(string message, string action = null, LogLevel logLevel = LogLevel.Information);
        Task LogTraceAsync(string message, string action = null);
        Task LogDebugAsync(string message, string action = null);
        Task LogInformationAsync(string message, string action = null);
        Task LogWarningAsync(string message, string action = null);
        Task LogErrorAsync(string message, string action = null);
    }
}
