using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryCancelledEvent(Guid DeliveryId) : DomainEventBase;
