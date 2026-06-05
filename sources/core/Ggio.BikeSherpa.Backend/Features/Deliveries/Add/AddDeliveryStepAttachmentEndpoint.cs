using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public class AddDeliveryStepAttachmentEndpoint(
     IMediator mediator
) : Endpoint<AttachmentRequest, Result>
{
     public override void Configure()
     {
          Post("/delivery/{deliveryId:guid}/step/{stepId:guid}/attachment");
          Policies("write:myDeliveries");
          AllowFileUploads();
          Description(x => 
               x.WithTags("delivery")
                    .Produces(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status404NotFound)
               );
     }

     public override async Task HandleAsync(AttachmentRequest req, CancellationToken ct)
     {
          var command = new AddDeliveryStepAttachmentCommand(
               req.DeliveryId,
               req.StepId,
               req.File
          );

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
