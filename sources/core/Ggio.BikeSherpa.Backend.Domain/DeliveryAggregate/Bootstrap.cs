using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Step;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddDeliveryAggregate()
          {
               services.AddScoped<IDeliveryFactory, DeliveryFactory>();
               services.AddScoped<IDeliveryDeleteEventHandler, DeliveryDeleteService>();
               services.AddScoped<IPricingStrategyService, PricingStrategyService>();
               services.AddScoped<IPricingStrategy, SimpleDeliveryStrategy>();
               services.AddScoped<IPricingStrategy, TourDeliveryStrategy>();
               services.AddScoped<IDeliveryChangeTimeService, DeliveryChangeTimeService>();
               return services;
          }

     }
}
