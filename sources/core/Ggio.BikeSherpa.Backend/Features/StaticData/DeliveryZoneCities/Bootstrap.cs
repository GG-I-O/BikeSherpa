using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.StaticData.DeliveryZoneCities.Check;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.DeliveryZoneCities;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureDeliveryZoneCitiesFeature()
          {
               services.AddTransient<IValidator<CheckDeliveryZoneCityQuery>, CheckDeliveryZoneCityQueryValidator>();
               return services;
          }
     }
}
