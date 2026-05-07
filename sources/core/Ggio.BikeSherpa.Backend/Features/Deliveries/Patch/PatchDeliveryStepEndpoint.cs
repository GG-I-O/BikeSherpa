using System.Globalization;
using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;

public class PatchDeliveryStepEndpoint(IMediator mediator) : Endpoint<PatchDeliveryRequest>
{
     public override void Configure()
     {
          Patch("/delivery/{deliveryId:guid}/step/{stepId:guid}");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
          Description(x => x.Accepts<PatchDeliveryRequest>("application/json-patch+json"));
     }

     public override async Task HandleAsync(PatchDeliveryRequest req, CancellationToken ct)
     {
          var requestPath = req.Patches.Operations[0].path.TrimStart('/');

          switch (requestPath)
          {
               case var _ when requestPath.Equals(nameof(DeliveryStep.Order), StringComparison.OrdinalIgnoreCase):
                    if (!int.TryParse(
                             req.Patches.Operations[0].value?.ToString(),
                             out var newOrder))
                    {
                         await Send.ToEndpointResult(Result.Error("Step order must be an integer"), ct);
                         return;
                    }
                    var orderCommand = new PatchDeliveryStepOrderCommand(req.DeliveryId, req.StepId, newOrder);
                    var orderResult = await mediator.Send(orderCommand, ct);
                    await Send.ToEndpointResult(orderResult, ct);
                    return;
               
               case var _ when requestPath.Equals(nameof(DeliveryStep.EstimatedDeliveryDate), StringComparison.OrdinalIgnoreCase):
                    if (!DateTimeOffset.TryParse(
                             req.Patches.Operations[0].value?.ToString(),
                             CultureInfo.InvariantCulture,
                             DateTimeStyles.RoundtripKind,
                             out var estimatedDeliveryDate))
                    {
                         await Send.ToEndpointResult(Result.Error("EstimatedDeliveryDate must be a valid ISO-8601 datetime"), ct);
                         return;
                    }
                    var timeCommand = new PatchDeliveryStepTimeCommand(req.DeliveryId, req.StepId, estimatedDeliveryDate);
                    var timeResult = await mediator.Send(timeCommand, ct);
                    await Send.ToEndpointResult(timeResult, ct);
                    return;
               case var _ when requestPath.Equals(nameof(DeliveryStep.Comment), StringComparison.OrdinalIgnoreCase):
                    var commentCommand = new PatchDeliveryStepCommentCommand(req.DeliveryId, req.StepId, req.Patches.Operations[0].value?.ToString());
                    var commentResult = await mediator.Send(commentCommand, ct);
                    await Send.ToEndpointResult(commentResult, ct);
                    return;
               default:
                    ThrowError("Invalid patch path");
                    break;
          }
     }
}
