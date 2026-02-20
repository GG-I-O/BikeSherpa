using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryCompletedEvent(object NewEntity) : DomainEventBase;
