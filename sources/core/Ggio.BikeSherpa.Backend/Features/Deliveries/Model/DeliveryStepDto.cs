using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Model;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public class DeliveryStepDto
{
     [UsedImplicitly]
     public required DeliveryStep Data { get; set; }
     public List<Link>? Links { get; set; }
}
