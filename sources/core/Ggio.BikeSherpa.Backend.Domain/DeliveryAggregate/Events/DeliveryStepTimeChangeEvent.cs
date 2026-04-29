using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public record DeliveryStepTimeChangeEvent(Guid DeliveryId) : DomainEventBase, IDeliveryEvent;
