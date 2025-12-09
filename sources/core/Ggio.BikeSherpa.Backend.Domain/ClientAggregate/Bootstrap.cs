using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.ClientAggregate;

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
