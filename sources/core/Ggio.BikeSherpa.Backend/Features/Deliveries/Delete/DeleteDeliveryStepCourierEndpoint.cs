using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;

public class DeleteDeliveryStepCourierEndpoint
(
     IMediator mediator
) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Delete("/delivery/{deliveryId:guid}/step/{stepId:guid}/courier");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new DeleteDeliveryStepCourierCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId")
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
