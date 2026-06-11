using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class ValidateDeliveryEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Put("/deliveries/{deliveryId:guid}/pending");
          AllowAnonymous();
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery").WithDescription("Validate a delivery to be able to start it and pass to pending status"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new ValidateDeliveryCommand(Route<Guid>("deliveryId"));
          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
