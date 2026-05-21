using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(Delivery), exclude: [
     nameof(Delivery.DomainEvents),
     nameof(Delivery.Steps),
     nameof(Delivery.Urgency),
     nameof(Delivery.PackingSize)
], Configuration = typeof(DeliveryCrudMapper))]
public partial record DeliveryCrud(string Urgency, string PackingSize, DateTimeOffset? LimitDate)
{
     public List<DeliveryStepDto> Steps { get; set; } = [];
     public string Urgency { get; set; } = Urgency;
     public string PackingSize { get; set; } = PackingSize;
     public DateTimeOffset? LimitDate { get; set; }= LimitDate;
}
