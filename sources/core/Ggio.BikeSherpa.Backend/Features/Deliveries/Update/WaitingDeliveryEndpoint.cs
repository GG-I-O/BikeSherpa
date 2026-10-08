using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class WaitingDeliveryEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Put("/deliveries/{deliveryId:guid}/waiting");
          AllowAnonymous();
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery").WithDescription("Put a delivery to the pending status to block it until it's validated"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new WaitingDeliveryCommand(Route<Guid>("deliveryId"));
          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
