using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryCompletedEvent(Guid DeliveryId) : DomainEventBase;
