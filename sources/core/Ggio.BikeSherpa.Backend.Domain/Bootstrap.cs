using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddBackendDomain()
          {
               services.AddCourseAggregate();
               return services;
          }
     }
}
