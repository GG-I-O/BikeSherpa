using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.Customers;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureCustomerFeature()
          {
               services.AddScoped<ICustomerLinks, CustomerLinks>();
               services.AddScoped<IValidator<AddCustomerCommand>, AddCustomerCommandValidator>();
               services.AddScoped<IValidator<UpdateClientCommand>, UpdateClientCommandValidator>();
               return services;
          }
     }
}
