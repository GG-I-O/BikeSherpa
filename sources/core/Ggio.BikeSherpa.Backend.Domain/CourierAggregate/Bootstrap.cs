using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddCourierAggregate()
          {
               services.AddScoped<ICourierFactory, CourierFactory>();
               services.AddScoped<ICourierDeleteEventHandler, CourierDeleteService>();
               return services;
          }
     }
}