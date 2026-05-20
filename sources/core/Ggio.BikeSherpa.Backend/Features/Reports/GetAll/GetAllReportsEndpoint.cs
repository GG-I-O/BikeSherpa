using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Reports.GetAll;

public class GetAllReportsEndpoint(
     IMediator mediator
     ) : EndpointWithoutRequest<List<Report>>
{
     public override void Configure()
     {
          Get("/reports/{customerId:guid}/{startDate:datetime}/{endDate:datetime}");
          Policies("read:deliveries");
          Description(x => x.WithTags("report"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var query = new GetAllReportsQuery(
               Route<Guid>("customerId"),
               Route<DateTime>("startDate"),
               Route<DateTime>("endDate")
          );

          var reports = await mediator.Send(query, ct);
          await Send.OkAsync(reports, ct);
     }
}
