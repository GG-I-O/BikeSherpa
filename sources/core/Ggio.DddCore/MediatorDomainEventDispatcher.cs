using Mediator;

namespace Ggio.DddCore;

public class MediatorDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
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
               var events = entity.DomainEvents.ToArray();
               if (clearEvents)
               {
                    entity.ClearDomainEvents();
               }

               foreach (var domainEvent in events)
                    await mediator.Publish(domainEvent).ConfigureAwait(false);
          }
     }
}
