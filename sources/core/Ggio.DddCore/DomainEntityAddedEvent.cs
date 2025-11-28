namespace Ggio.DddCore;

public record DomainEntityAddedEvent(object NewEntity) : DomainEventBase;
