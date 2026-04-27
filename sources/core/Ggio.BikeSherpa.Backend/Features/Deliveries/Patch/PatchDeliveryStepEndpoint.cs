using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public class PatchDeliveryStepEndpoint(IMediator mediator): Endpoint<PatchDeliveryRequest>
{
     public override void Configure()
     {
          Patch("/delivery/{deliveryId:guid}/step/{stepId:guid}");
          Policies("write:deliveries");
          Description(x=> x.WithTags("delivery"));
          Description(x => x.Accepts<PatchDeliveryRequest>("application/json-patch+json"));
     }
     
     public override async Task HandleAsync(PatchDeliveryRequest req, CancellationToken ct)
     {
          var requestPath = req.Patches.Operations[0].path.TrimStart('/');

          switch (requestPath)
          {
               case var _ when requestPath.Equals(nameof(DeliveryStep.Order), StringComparison.OrdinalIgnoreCase):
                    var orderCommand = new PatchDeliveryStepOrderCommand(req);
                    var orderResult = await mediator.Send(orderCommand, ct);
                    await Send.ToEndpointResult(orderResult, ct);
                    return;
               case var _ when requestPath.Equals(nameof(DeliveryStep.EstimatedDeliveryDate), StringComparison.OrdinalIgnoreCase):
                    var timeCommand = new PatchDeliveryStepTimeCommand(req);
                    var timeResult = await mediator.Send(timeCommand, ct);
                    await Send.ToEndpointResult(timeResult, ct);
                    return;
               default:
                    ThrowError("Invalid patch path");
                    break;
          }
     }
}
