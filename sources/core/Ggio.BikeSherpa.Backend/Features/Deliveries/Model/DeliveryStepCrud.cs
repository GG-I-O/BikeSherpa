using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(DeliveryStep), exclude: [nameof(DeliveryStep.ParentDelivery), nameof(DeliveryStep.DomainEvents)])]
public partial record DeliveryStepCrud;
