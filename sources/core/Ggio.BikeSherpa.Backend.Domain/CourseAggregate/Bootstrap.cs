using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Domain.CourseAggregate;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddCourseAggregate()
          {
               services.AddScoped<ICourseFactory, CourseFactory>();
               return services;
          }
          
     }
}
