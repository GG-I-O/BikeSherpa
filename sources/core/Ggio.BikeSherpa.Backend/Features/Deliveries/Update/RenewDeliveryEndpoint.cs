using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class RenewDeliveryEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Put("/deliveries/{deliveryId:guid}/renew");
          AllowAnonymous();
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery").WithDescription("Renew a delivery to be able to start it and pass to pending status"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new RenewDeliveryCommand(Route<Guid>("deliveryId"));
          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
