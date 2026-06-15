using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(Delivery), nameof(Delivery.DomainEvents), nameof(Delivery.Steps), nameof(Delivery.Urgency), Configuration = typeof(DeliveryCrudMapper))]
public partial record DeliveryCrud
{
     public List<DeliveryStepDto> Steps { get; set; } = [];
     public string Urgency { get; set; } = string.Empty;
     public DateTimeOffset? LimitDate { get; set; }
}
