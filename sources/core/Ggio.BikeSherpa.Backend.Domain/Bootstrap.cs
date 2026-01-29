using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddBackendDomain()
          {
               services.AddCourseAggregate();
               services.AddCustomerAggregate();
               services.AddCourierAggregate();
               return services;
          }
     }
}
