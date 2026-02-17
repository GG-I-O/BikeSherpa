using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

[Facet(typeof(Urgency))]
public partial class UrgencyEntity
{
     public int Id { get; set; }
}
