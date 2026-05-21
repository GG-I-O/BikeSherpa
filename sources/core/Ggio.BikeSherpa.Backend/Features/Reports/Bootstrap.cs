using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Reports.Get;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.BikeSherpa.Backend.Features.Reports;

public static class Bootstrap
{
     extension(IServiceCollection services)
     {
          public IServiceCollection ConfigureReportFeature()
          {
               services.AddScoped<IReportService, ReportService>();
               services.AddScoped<IValidator<GetReportQuery>, GetReportQueryValidator>();
               return services;
          }
     }
}
