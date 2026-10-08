using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryWaitingEvent(Guid DeliveryId) : DomainEventBase, IDeliveryEvent;
