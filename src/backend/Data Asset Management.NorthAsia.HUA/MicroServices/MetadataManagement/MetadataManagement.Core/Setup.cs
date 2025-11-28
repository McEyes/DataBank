using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using Furion;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MetadataManagement.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSqlsugarSetup();
            services.AddDapper(App.Configuration.GetConnectionString("DataAsset"), SqlProvider.Npgsql);
        }
    }
}
