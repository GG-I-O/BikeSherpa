using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain;

public static class Bootstrap
{
     public static IServiceCollection AddBackendDomain(this IServiceCollection services)
     {
          services.AddDeliveryAggregate()
               .AddCustomerAggregate()
               .AddCourierAggregate()
               .AddSharedKernel();

          return services;
     }
}
