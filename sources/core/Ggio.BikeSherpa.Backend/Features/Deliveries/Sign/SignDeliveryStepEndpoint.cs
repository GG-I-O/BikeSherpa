using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Sign;

public record SignDeliveryStepRequest
{
     [FromRoute]
     public required Guid DeliveryId { get; init; }
     
     [FromRoute]
     public required Guid StepId { get; init; }
     
     public required IFormFile Signature { get; set; }
     
     public required string DomainType { get; set; }
     
     public required string Receiver { get; set; }
}

public class SignDeliveryStepEndpoint(
     IMediator mediator
     ) : Endpoint<SignDeliveryStepRequest, Result>
{
     public override void Configure()
     {
          Put("/delivery/{deliveryId:guid}/step/{stepId:guid}/sign");
          Policies("CanSignStep");
          AllowFileUploads();
          Description(x => 
               x.WithTags("delivery")
                    .Produces(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status404NotFound)
          );
     }

     public override async Task HandleAsync(SignDeliveryStepRequest req, CancellationToken ct)
     {
          var command = new SignDeliveryStepCommand(
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
