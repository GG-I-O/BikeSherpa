namespace Ggio.DddCore;

public record DeliveryCompletedEvent(object NewEntity) : DomainEventBase;
