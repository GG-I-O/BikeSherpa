using FluentValidation;
using Ggio.BikeSherpa.Backend.Features.Reports.Customer;
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
               services.AddScoped<IValidator<ExportReportCommand>, ExportReportCommandValidator>();
               services.AddScoped<IValidator<Courier.GetReportQuery>, Courier.GetReportQueryValidator>();
               return services;
          }
     }
}
