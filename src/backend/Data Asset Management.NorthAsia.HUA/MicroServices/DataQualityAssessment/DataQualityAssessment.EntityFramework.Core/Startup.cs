using DataQualityAssessment.Core.DbContextLocators;
using Furion;
using Microsoft.Extensions.DependencyInjection;

namespace DataQualityAssessment.EntityFramework.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseAccessor(options =>
        {
            options.AddDbPool<DefaultDbContext>();
            options.AddDbPool<DataAssetDbContext, DataAssetDbContextLocator>();
        }, "DataQualityAssessment.Database.Migrations");
    }
}
