using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Couriers.Add;
using Ggio.BikeSherpa.Backend.Features.Couriers.Services;
using Ggio.BikeSherpa.Backend.Features.Couriers.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.Couriers;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureCourierFeature()
          {
               services.AddScoped<ICourierLinks, CourierLinks>();
               services.AddScoped<IValidator<AddCourierCommand>, AddCourierCommandValidator>();
               services.AddScoped<IValidator<UpdateCourierCommand>, UpdateCourierCommandValidator>();
               return services;
          }
     }
}
