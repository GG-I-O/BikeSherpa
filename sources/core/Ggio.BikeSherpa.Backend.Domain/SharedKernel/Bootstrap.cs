using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.SharedKernel;

public static class Bootstrap
{
     public static IServiceCollection AddSharedKernel(this IServiceCollection services)
     {
          services.AddScoped<IVatService, VatService>();
          return services;
     }
}
