using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Clients.Add;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.Clients;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureClientFeature() => services.AddScoped<IValidator<AddClientCommand>, AddClientCommandValidator>();
     }
}
