using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Customer;

public class GetReportEndpoint(
     IMediator mediator
) : Endpoint<GetReportRequest, Report>
{
     public override void Configure()
     {
          Get("/reports/customer/{customerId:guid}");
          Policies("read:reports");
          Description(x => x.WithTags("report").WithName("GetCustomerReport"));
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
