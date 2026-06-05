using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Events;

public record CustomerCodeChanged(Guid CustomerId, string Code) : DomainEventBase;
