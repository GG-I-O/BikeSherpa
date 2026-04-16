namespace Ggio.DddCore;

public record AggregateRootDeletedEvent(object DeletedAggregate) : DomainEventBase;
