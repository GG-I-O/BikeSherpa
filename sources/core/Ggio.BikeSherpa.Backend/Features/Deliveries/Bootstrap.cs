using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureDeliveryFeature() => services.AddScoped<IValidator<AddDeliveryCommand>, AddDeliveryCommandValidator>();
     }
}
