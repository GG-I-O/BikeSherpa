using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class UpdateDeliveryEndpoint(IMediator mediator) : Endpoint<DeliveryCrud>
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(DeliveryCrud req, CancellationToken ct)
     {
          var command = new UpdateDeliveryCommand(
                Route<Guid>("deliveryId"),
                req.PricingStrategy,
                req.Status,
                req.Code,
                req.CustomerId,
                req.Urgency,
                req.TotalPrice,
                req.ReportId,
                req.Steps,
                req.Details,
                req.PackingSize,
                req.InsulatedBox,
                req.ExactTime,
                req.ReturnJourney,
                req.ContractDate,
                req.StartDate
           );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
