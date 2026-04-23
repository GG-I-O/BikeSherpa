using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(Delivery), exclude: [nameof(Delivery.DomainEvents), nameof(Delivery.Steps)], Configuration = typeof(DeliveryCrudMapper))]
public partial record DeliveryCrud
{
     public List<DeliveryStepDto> Steps { get; set; } = [];
}
