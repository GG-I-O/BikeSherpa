using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryRenewedEvent(Guid DeliveryId) : DomainEventBase, IDeliveryEvent;
