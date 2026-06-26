using FastEndpoints;
using Ggio.BikeSherpa.Backend.Infrastructure.Storage;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Courier;

public class GetReportEndpoint(
     IMediator mediator,
     IExportSaveService exportSaveService,
     ILogger<GetReportEndpoint> logger
) : Endpoint<GetReportRequest>
{
     public override void Configure()
     {
          Get("/reports/courier/{courierId:guid}");
          Policies("read:reports");
          Description(x => x.WithTags("report")
               .WithName("GetCourierReport")
               .Produces<string>());
     }

     public override async Task HandleAsync(GetReportRequest req, CancellationToken ct)
     {
          var query = new GetReportQuery(
               req.CourierId,
               req.StartDate,
               req.EndDate
          );

          var reportFile = await mediator.Send(query, ct);


          var ret = await exportSaveService.SaveCourierReportAsync(reportFile.FileName,
               reportFile.Content,
               reportFile.ContentType,
               ct);

          logger.LogDebug("Rapport {FileName} enregistré avec succès à l'url {Url}", reportFile.FileName, ret);
          await Send.OkAsync(ret, ct);

     }
}
