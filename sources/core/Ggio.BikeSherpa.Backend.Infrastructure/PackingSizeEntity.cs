using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

[Facet(typeof(PackingSize))]
public partial class PackingSizeEntity
{
     public int Id { get; set; }
}
