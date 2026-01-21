namespace Ggio.DddCore;

public record DomainEntityDeletedEvent(object NewEntity) : DomainEventBase;
