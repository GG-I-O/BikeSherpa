using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Reports.GetAll;

public class GetAllReportsEndpoint(
     IMediator mediator
     ) : Endpoint<GetAllReportsRequest, List<Report>>
{
     public override void Configure()
     {
          Get("/reports");
          Policies("read:deliveries");
          Description(x => x.WithTags("report"));
     }

     public override async Task HandleAsync(GetAllReportsRequest req, CancellationToken ct)
     {
          var query = new GetAllReportsQuery(
               req.CustomerId,
               req.StartDate,
               req.EndDate
          );

          var reports = await mediator.Send(query, ct);
          await Send.OkAsync(reports, ct);
     }
}
