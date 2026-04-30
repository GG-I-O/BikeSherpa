using Ggio.BikeSherpa.Backend.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTests;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
     private const string TestConnectionString =
          "Host=localhost;Port=5433;Database=ggio_tests_db;Username=postgres;Password=postgres";

     override protected void ConfigureWebHost(IWebHostBuilder builder)
     {
          builder.UseEnvironment("Development");

          builder.ConfigureServices(services =>
          {
               var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BackendDbContext>));

               if (dbContextDescriptor is not null)
               {
                    services.Remove(dbContextDescriptor);
               }

               services.AddDbContext<BackendDbContext>(options =>
                    options.UseNpgsql(TestConnectionString));
          });
     }
}
