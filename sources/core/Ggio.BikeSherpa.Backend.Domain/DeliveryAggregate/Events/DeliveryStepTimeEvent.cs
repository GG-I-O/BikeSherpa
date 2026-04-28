using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryStepTimeEvent(Guid DeliveryId) : DomainEventBase, IDeliveryEvent;
