namespace Ggio.DddCore;

public record AggregateRootAddedEvent(object NewAggregate) : DomainEventBase;
