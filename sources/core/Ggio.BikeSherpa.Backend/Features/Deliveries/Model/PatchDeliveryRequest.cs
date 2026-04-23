using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using JetBrains.Annotations;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[UsedImplicitly]
public class PatchDeliveryRequest
{
     [RouteParam]
     public Guid DeliveryId { get; set; }

     [RouteParam]
     public Guid StepId { get; set; }

     [FromBody]
     public JsonPatchDocument<DeliveryStep> Patches { get; set; } = null!;
}