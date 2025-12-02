using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class BackendDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BackendDbContext>
{
     public BackendDbContext CreateDbContext(string[] args)
     {
          var configurationRoot = new ConfigurationBuilder()
               .AddUserSecrets<BackendDbContextDesignTimeFactory>();

          var configuration = configurationRoot.Build();
          // DbPassword & DesignConnectionString in user secrets
          var conStrBuilder = new NpgsqlConnectionStringBuilder(
               configuration["DesignConnectionString"])
          {
               Password = configuration["DbPassword"]
          };
          var connectionString = conStrBuilder.ConnectionString;
          Guard.Against.NullOrEmpty(connectionString);

          var options = new DbContextOptionsBuilder<BackendDbContext>()
               .UseNpgsql(connectionString);


          return new BackendDbContext(options.Options);
     }
}
