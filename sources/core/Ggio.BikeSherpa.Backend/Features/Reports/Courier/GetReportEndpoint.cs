using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Courier;

public class GetReportEndpoint(
     IMediator mediator
) : Endpoint<GetReportRequest>
{
     public override void Configure()
     {
          Get("/reports/courier/{courierId:guid}");
          Policies("read:reports");
          Description(x => x.WithTags("report"));
     }

     public override async Task HandleAsync(GetReportRequest req, CancellationToken ct)
     {
          var query = new GetReportQuery(
               req.CourierId,
               req.StartDate,
               req.EndDate
          );

          var reportFile = await mediator.Send(query, ct);
          await Send.BytesAsync(reportFile.Content, reportFile.FileName, reportFile.ContentType, cancellation: ct);
     }
}
