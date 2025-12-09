using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddClientAggregate()
          {
               services.AddScoped<IClientFactory, ClientFactory>();
               return services;
          }
     }
}
