using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryValidatedEvent(Guid DeliveryId) : DomainEventBase, IDeliveryEvent;
