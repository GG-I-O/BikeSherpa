using Ggio.BikeSherpa.Backend.Model;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Model;

public record CourierDto : IHateoasLinks
{
     [UsedImplicitly]
     public required CourierCrud Data { get; init; }
     public List<Link>? Links { get; set; }
}