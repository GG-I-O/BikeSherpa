using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class UpdateDeliveryStepCompletionEndpoint(IMediator mediator) : Endpoint<UpdateDeliveryStepCompletionRequest>
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}/step/{stepId:guid}/complete");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(UpdateDeliveryStepCompletionRequest req, CancellationToken ct)
     {
          var command = new UpdateDeliveryStepCompletionCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId"),
               req.Completed
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
