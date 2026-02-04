using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.StepAggregate;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddDeliveryAggregate()
          {
               services.AddScoped<IStepFactory, StepFactory>();
               services.AddScoped<IStepDeleteEventHandler, StepDeleteService>();
               return services;
          }
          
     }
}
