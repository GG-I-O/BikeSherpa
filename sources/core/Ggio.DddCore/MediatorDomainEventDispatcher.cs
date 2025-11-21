using Mediator;
using Microsoft.Extensions.Logging;

namespace Ggio.DddCore;

public class MediatorDomainEventDispatcher(IMediator mediator, ILogger<MediatorDomainEventDispatcher> logger) : IDomainEventDispatcher
{
     public async Task DispatchEventsAsync(IEnumerable<IHasDomainEvents> entitiesWithEvents)
     {
          await DispatchInternal(entitiesWithEvents, false);
     }

     public async Task DispatchAndClearEventsAsync(IEnumerable<IHasDomainEvents> entitiesWithEvents)
     {
          await DispatchInternal(entitiesWithEvents);
     }

     private async Task DispatchInternal(IEnumerable<IHasDomainEvents> entitiesWithEvents, bool clearEvents = true)
     {

          foreach (var entity in entitiesWithEvents)
          {
               if (entity is IHasDomainEvents hasDomainEvents)
               {
                    var events = hasDomainEvents.DomainEvents.ToArray();
                    if (clearEvents)
                    {
                         hasDomainEvents.ClearDomainEvents();
                    }

                    foreach (var domainEvent in events)
                         await mediator.Publish(domainEvent).ConfigureAwait(false);
               }
               else
               {
                    logger.LogError(
                         "Entity of type {EntityType} does not inherit from {BaseType}. Unable to clear domain events.",
                         entity.GetType().Name,
                         nameof(IHasDomainEvents));
               }
          }
     }
}
