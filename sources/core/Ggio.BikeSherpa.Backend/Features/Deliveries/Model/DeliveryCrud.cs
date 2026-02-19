using Facet;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

[Facet(typeof(Delivery), exclude:nameof(Delivery.DomainEvents))]
public partial record DeliveryCrud;
