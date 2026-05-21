using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Get;

public class GetReportEndpoint(
     IMediator mediator
     ) : Endpoint<GetReportRequest, Report>
{
     public override void Configure()
     {
          Get("/reports");
          Policies("read:reports");
          Description(x => x.WithTags("report"));
     }

     public override async Task HandleAsync(GetReportRequest req, CancellationToken ct)
     {
          var query = new GetReportQuery(
               req.CustomerId,
               req.StartDate,
               req.EndDate
          );

          var reports = await mediator.Send(query, ct);
          await Send.OkAsync(reports, ct);
     }
}
