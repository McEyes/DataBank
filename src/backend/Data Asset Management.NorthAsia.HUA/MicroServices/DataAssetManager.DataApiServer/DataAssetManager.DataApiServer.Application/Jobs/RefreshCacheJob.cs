using DataAssetManager.DataTableServer.Application;

using Furion.Schedule;
using Furion.TimeCrontab;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Flow.Enums;
using ITPortal.Core.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DataAssetManager.DataApiServer.Application.Jobs
{
    /// <summary>
    /// 每分钟检查缓存情况
    /// </summary>
    //[Cron("1,* * * *")]
    [Cron("0/10 * * * * ?", CronStringFormat.WithSeconds)]
    public class RefreshCacheJob : Furion.Schedule.IJob
    {
        private readonly ILogger<RefreshCacheJob> _logger;
        private readonly IDistributedCacheService _cache;
        private static bool _IsRunning = false;
        public RefreshCacheJob(ILogger<RefreshCacheJob> logger, IDistributedCacheService cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            try
            {
                if (_IsRunning)
                {
                    _logger.LogInformation($"{context} is running Refresh Cache job, skip");
                    return;
                }
                _logger.LogInformation($"{context} running Refresh Cache job");
                _IsRunning = true;
                var apiRoteList = this._cache.HashGetAll<RouteInfo>(DataAssetManagerConst.DataApis_HashKey);
                if (apiRoteList == null || apiRoteList.Count <= 0)
                {
                    _logger.LogInformation($"{context} DataApis_HashKey is empty,run init");
                    IDataApiService apiService = context.ServiceProvider.GetService<IDataApiService>();
                    await apiService.InitRoutes(true);
                }

                var userClientList = this._cache.HashGetAll<List<string>>(DataAssetManagerConst.DataTable_UserHashKey);
                if (userClientList == null || userClientList.Count <= 0)
                {
                    _logger.LogInformation($"{context} DataTable_UserHashKey is empty,run init");
                    var clientsService = context.ServiceProvider.GetService<IAssetClientsService>();
                    await clientsService.InitClientScopes(true);
                }
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
