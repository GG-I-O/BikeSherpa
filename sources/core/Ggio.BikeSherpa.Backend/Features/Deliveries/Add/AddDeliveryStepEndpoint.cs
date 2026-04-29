using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public class AddDeliveryStepEndpoint(IMediator mediator) : Endpoint<DeliveryStepCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/delivery/{deliveryId:guid}/step");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(DeliveryStepCrud req, CancellationToken ct)
     {
          var command = new AddDeliveryStepCommand(
               Route<Guid>("deliveryId"),
               req.StepType,
               req.StepAddress,
               req.Comment
          );

          var result = await mediator.Send(command, ct);

          await Send.CreatedAtAsync<GetDeliveryEndpoint>(result.Value, new AddResult<Guid>(result.Value), cancellation: ct);
     }
}
