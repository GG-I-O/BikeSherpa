using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class UpdateDeliveryStepOrderEndpoint(IMediator mediator) : Endpoint<UpdateDeliveryStepOrderRequest>
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}/step/{stepId:guid}/changeOrder");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(UpdateDeliveryStepOrderRequest req, CancellationToken ct)
     {
          var command = new UpdateDeliveryStepOrderCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId"),
               req.Increment
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
