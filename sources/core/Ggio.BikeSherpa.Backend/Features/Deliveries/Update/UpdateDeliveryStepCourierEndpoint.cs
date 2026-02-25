using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class UpdateDeliveryStepCourierEndpoint(IMediator mediator) : Endpoint<UpdateDeliveryStepCourierRequest>
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}/step/{stepId:guid}/courier");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(UpdateDeliveryStepCourierRequest req, CancellationToken ct)
     {
          var command = new UpdateDeliveryStepCourierCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId"),
               req.CourierId
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
