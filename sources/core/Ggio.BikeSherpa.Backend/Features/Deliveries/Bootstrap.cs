using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Courses.Add;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.Courses;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureCourseFeature() => services.AddScoped<IValidator<AddCourseCommand>, AddCourseCommandValidator>();
     }
}
