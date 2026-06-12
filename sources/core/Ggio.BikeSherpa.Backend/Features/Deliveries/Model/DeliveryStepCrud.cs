using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(DeliveryStep), nameof(DeliveryStep.ParentDelivery), nameof(DeliveryStep.DomainEvents), nameof(DeliveryStep.PackingSize))]
public partial record DeliveryStepCrud
{
     public string PackingSize { get; set; } = string.Empty;
}
