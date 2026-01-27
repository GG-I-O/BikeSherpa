using Ggio.BikeSherpa.Backend.Model;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public record DeliveryDto: IHateoasLinks
{
     [UsedImplicitly]
     public required DeliveryCrud Data { get; init; }
     public List<Link>? Links { get; set; }
}