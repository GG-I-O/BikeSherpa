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
          var connectionString = configuration["DesignConnectionString"];
          Guard.Against.NullOrEmpty(connectionString);

          var options = new DbContextOptionsBuilder<BackendDbContext>()
               .UseNpgsql(connectionString);

          return new BackendDbContext(options.Options);
     }
}
