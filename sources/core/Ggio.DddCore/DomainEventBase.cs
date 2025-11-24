namespace Ggio.DddCore;

/// <summary>
///      A base type for domain events. Depends on Mediator INotification.
///      Includes DateOccurred which is set on creation.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
     public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
