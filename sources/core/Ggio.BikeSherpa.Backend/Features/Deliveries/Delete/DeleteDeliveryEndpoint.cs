using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;

public class DeleteDeliveryEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Delete("/delivery/{deliveryId:guid}");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var command = new DeleteDeliveryCommand(
               Route<Guid>("deliveryId"));
          
          var result = await mediator.Send(command, ct);
          await Send.ToEndpointWithoutRequestResult(result, ct);
     }
}
