namespace Ggio.DddCore;

/// <summary>
///      A simple interface for sending domain events. Can use MediatR or any other implementation.
/// </summary>
public interface IDomainEventDispatcher
{
     Task DispatchEventsAsync(IEnumerable<IHasDomainEvents> entitiesWithEvents);
     Task DispatchAndClearEventsAsync(IEnumerable<IHasDomainEvents> entitiesWithEvents);
}
