using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public class AddDeliveryStepSignatureEndpoint(
     IMediator mediator
     ) : Endpoint<SignatureRequest, Result>
{
     public override void Configure()
     {
          Post("/delivery/{deliveryId:guid}/step/{stepId:guid}/signature");
          Policies("write:myDeliveries");
          AllowFileUploads();
          Description(x => 
               x.WithTags("delivery")
                    .Produces(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status404NotFound)
          );
     }

     public override async Task HandleAsync(SignatureRequest req, CancellationToken ct)
     {
          var command = new AddDeliveryStepSignatureCommand(
               req.DeliveryId,
               req.StepId,
               req.Signature,
               req.DomainType,
               req.Receiver
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
