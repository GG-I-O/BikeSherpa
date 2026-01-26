using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddDeliveryAggregate()
          {
               services.AddScoped<IDeliveryFactory, DeliveryFactory>();
               return services;
          }
          
     }
}
