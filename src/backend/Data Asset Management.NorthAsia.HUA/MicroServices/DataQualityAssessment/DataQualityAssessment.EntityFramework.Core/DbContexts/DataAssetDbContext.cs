using DataQualityAssessment.Core.DbContextLocators;
using DataQualityAssessment.Core.Entities.DataAsset;
using Furion;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;


namespace DataQualityAssessment.EntityFramework.Core;

//[AppDbContext("DataAsset", DbProvider.Npgsql)]
public class DataAssetDbContext : AppDbContext<DataAssetDbContext>
{
    public DataAssetDbContext(DbContextOptions<DataAssetDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionStringPlaintext = App.Configuration.GetConnectionString(DataAssetDbProperties.ConnectionStringName);
        base.OnConfiguring(optionsBuilder.UseNpgsql(connectionStringPlaintext));
    }
}
