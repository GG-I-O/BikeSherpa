using FluentValidation;
using Ggio.BikeSherpa.BackendSaaS.Features.Courses.Add;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureCourseFeature() => services.AddScoped<IValidator<AddCourseCommand>, AddCourseCommandValidator>();
     }
}
