using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryCancelledEvent(object NewEntity) : DomainEventBase;
