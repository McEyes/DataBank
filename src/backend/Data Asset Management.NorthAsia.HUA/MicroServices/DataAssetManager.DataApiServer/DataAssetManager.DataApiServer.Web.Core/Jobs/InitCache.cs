using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataApiServer.Web.Core.Extensions;
using DataAssetManager.DataTableServer.Application;

using Furion;

using ITPortal.Core;

using Microsoft.Extensions.Hosting;

using StackExchange.Profiling.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Web.Core.Jobs
{
    public class InitCache : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                var hostUrl = App.GetConfig<string>("RemoteApi:DataAssetHostUrl");
                if (!hostUrl.IsNullOrWhiteSpace()) DataAssetManagerConst.HostUrl = hostUrl;
                //启动数据
                Task.Run(() =>
                {
                    App.GetService<IDataApiService>().InitRoutes(true);
                });
                Task.Run(() =>
                {
                    App.GetService<IDataTableService>().InitRedisHash();
                });
                Task.Run(() =>
                {
                    App.GetService<IDataTableService>().InitTableUserFromCache(true);
                });
                Task.Run(() =>
                {
                    App.GetService<IAssetClientsService>().InitClientScopes(true);
                });
            }
            return Task.CompletedTask;
        }

    }
}
