using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public class AddDeliveryStepCourierEndpoint(
     IMediator mediator
     ) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Post("/delivery/{deliveryId:guid}/step/{stepId:guid}/courier/{courierId:guid}");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new AddDeliveryStepCourierCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId"),
               Route<Guid>("courierId")
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
