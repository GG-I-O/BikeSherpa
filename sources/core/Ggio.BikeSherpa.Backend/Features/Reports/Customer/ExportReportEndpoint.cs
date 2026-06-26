using FastEndpoints;
using Ggio.BikeSherpa.Backend.Infrastructure.Storage;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Customer;

public record ExportReportRequest(Guid CustomerId, DateTimeOffset From, DateTimeOffset To);

public class ExportReportEndpoint(
     IMediator mediator,
     IExportSaveService exportSaveService,
     ILogger<ExportReportEndpoint> logger
) : Endpoint<ExportReportRequest, string>
{
     public override void Configure()
     {
          Get("/reports/customer/{customerId:guid}/export");
          Policies("read:reports");
          Description(x => x.WithTags("report")
               .WithName("ExportCustomerReport")
               .Produces<string>());
     }

     public override async Task HandleAsync(ExportReportRequest request, CancellationToken cancellationToken)
     {
          var command = new ExportReportCommand(
               request.CustomerId,
               request.From,
               request.To
          );

          var result = await mediator.Send(command, cancellationToken);

          var url = await exportSaveService.SaveCustomerReportAsync(
               result.FileName,
               result.Content,
               result.ContentType,
               cancellationToken
          );

          logger.LogDebug("Rapport {FileName} enregistré avec succès à l'url {Url}", result.FileName, url);
          await Send.OkAsync(url, cancellationToken);
     }
}
