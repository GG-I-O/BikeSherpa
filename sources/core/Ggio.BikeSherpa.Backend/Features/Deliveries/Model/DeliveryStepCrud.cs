using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(DeliveryStep), nameof(DeliveryStep.ParentDelivery), nameof(DeliveryStep.DomainEvents), nameof(DeliveryStep.PackingSize), nameof(DeliveryStep.StepZone), Configuration = typeof(DeliveryStepCrudMapper))]
public partial record DeliveryStepCrud
{
     public string PackingSize { get; set; } = string.Empty;
     public string StepZone { get; set; }
}
