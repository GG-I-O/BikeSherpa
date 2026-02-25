using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class CancelDeliveryEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}/cancel");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new CancelDeliveryCommand(
               Route<Guid>("deliveryId")
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
