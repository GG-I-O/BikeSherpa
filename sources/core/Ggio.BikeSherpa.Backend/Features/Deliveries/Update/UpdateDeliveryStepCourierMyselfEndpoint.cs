using System.Security.Claims;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public class UpdateDeliveryStepCourierMyselfEndpoint(IMediator mediator) : EndpointWithoutRequest
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}/step/{stepId:guid}/courier/myself");
          Policies("write:myDeliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
          
          var command = new UpdateDeliveryStepCourierMyselfCommand(
               Route<Guid>("deliveryId"),
               Route<Guid>("stepId"),
               userEmail!
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
