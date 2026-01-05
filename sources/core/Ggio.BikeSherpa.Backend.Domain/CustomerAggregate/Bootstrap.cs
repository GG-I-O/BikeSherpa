using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddCustomerAggregate()
          {
               services.AddScoped<ICustomerFactory, CustomerFactory>();
               services.AddScoped<ICustomerTrash, CustomerTrash>();
               return services;
          }
     }
}
