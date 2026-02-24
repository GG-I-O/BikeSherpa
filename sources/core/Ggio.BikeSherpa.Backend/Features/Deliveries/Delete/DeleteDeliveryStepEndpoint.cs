using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;

public class DeleteDeliveryStepEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Delete("/delivery/{deliveryId:guid}/{stepId:guid}");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new DeleteDeliveryStepCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId"));

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointWithoutRequestResult(result, ct);
     }
}
