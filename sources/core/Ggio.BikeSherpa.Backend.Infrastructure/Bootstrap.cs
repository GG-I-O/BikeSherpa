using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Infrastructure.Interceptors;
using Ggio.BikeSherpa.Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ggio.DddCore.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public void AddBackendInfrastructure(IConfiguration configuration)
          {
               var connectionString = configuration["ConnectionString"];
               services.AddDddDbContext<BackendDbContext>((_, options) =>
                    options.UseNpgsql(connectionString)
                         .AddInterceptors(new DateInterceptor())
               );
               services.AddScoped<IDeliveryZoneRepository, DeliveryZoneRepository>();
               services.AddScoped<IPackingSizeRepository, PackingSizeRepository>();
               services.AddScoped<IUrgencyRepository, UrgencyRepository>();
          }
     }
}
