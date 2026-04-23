using FastEndpoints;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public class PatchDeliveryStepTimeEndpoint(IMediator mediator): Endpoint<PatchDeliveryRequest>
{
     public override void Configure()
     {
          Patch("/delivery/{deliveryId:guid}/step/{stepId:guid}/time");
          Policies("write:deliveries");
          Description(x=> x.WithTags("delivery"));
          Description(x => x.Accepts<PatchDeliveryRequest>("application/json-patch+json"));
     }
     
     public override async Task HandleAsync(PatchDeliveryRequest req, CancellationToken ct)
     {
          var command = new PatchDeliveryStepTimeCommand(req);

          var result = await mediator.Send(command, ct);
          await Send.ToEndpointResult(result, ct);
     }
}
