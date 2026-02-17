using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

[Facet(typeof(DeliveryZone))]
public partial class DeliveryZoneEntity
{
     public int Id { get; set; }
}
