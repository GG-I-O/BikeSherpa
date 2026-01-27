using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureDeliveryFeature()
          {
               services.AddScoped<IDeliveryLinks, DeliveryLinks>();
               services.AddScoped<IValidator<AddDeliveryCommand>, AddDeliveryCommandValidator>();
               services.AddScoped<IValidator<UpdateDeliveryCommand>, UpdateDeliveryCommandValidator>();
               return services;
          }
     }
}
