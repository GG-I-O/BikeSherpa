using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(Delivery), exclude: [
     nameof(Delivery.DomainEvents),
     nameof(Delivery.Steps),
     nameof(Delivery.Urgency),
     nameof(Delivery.PackingSize)
], Configuration = typeof(DeliveryCrudMapper))]
public partial record DeliveryCrud
{
     public List<DeliveryStepDto> Steps { get; set; } = [];
     public required string Urgency { get; set; }
     public required string PackingSize { get; set; }
     public DateTimeOffset? LimitDate { get; set; }
}
