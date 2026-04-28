using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class UpdateDeliveryStepTimeEndpoint(IMediator mediator) : Endpoint<UpdateDeliveryStepTimeRequest>
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}/step/{stepId:guid}/changeTime");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(UpdateDeliveryStepTimeRequest req, CancellationToken ct)
     {
          var command = new UpdateDeliveryStepTimeCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId"),
               req.Date
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
