using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryStartedEvent(Guid DeliveryId) : DomainEventBase;
