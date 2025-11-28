using DataQualityAssessment.EntityFramework.Core.Dappers;
using Furion;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataQualityAssessment.EntityFramework.Core;

//[AppDbContext("DataQualityAssessment", DbProvider.Npgsql)]
public class DefaultDbContext : AppDbContext<DefaultDbContext>
{
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionStringPlaintext = App.Configuration.GetConnectionString(DefaultDbProperties.ConnectionStringName);
        base.OnConfiguring(optionsBuilder.UseNpgsql(connectionStringPlaintext));
    }
}
