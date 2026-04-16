using Mediator;

namespace Ggio.DddCore;

public class MediatorDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
     public async Task DispatchEventsAsync(IEnumerable<IHasDomainEvents> entitiesWithEvents)
     {
          await DispatchInternal(entitiesWithEvents, false);
     }

     public async Task DispatchEventsToPostTransactionalHandlersAsync(IEnumerable<IHasDomainEvents> entitiesWithEvents)
     {
          await DispatchInternal(entitiesWithEvents, isPostTransaction: true);
     }

     private async Task DispatchInternal(IEnumerable<IHasDomainEvents> entitiesWithEvents, bool clearEvents = true, bool isPostTransaction = false)
     {
          foreach (var entity in entitiesWithEvents)
          {
               var events = entity.DomainEvents.ToArray();
               if (clearEvents)
               {
                    entity.ClearDomainEvents();
               }

               var exceptionList = new List<Exception>();
               foreach (var domainEvent in events)
               {
                    try
                    {
                         await mediator.Publish(domainEvent).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                         if (isPostTransaction)
                         {
                              exceptionList.Add(e);
                              continue;
                         }

                         throw new InvalidOperationException($"Error dispatching event {domainEvent.GetType().Name}: {e.Message}", e);
                    }
               }

               if (isPostTransaction && exceptionList.Any())
               {
                    throw new AggregateException("Errors occurred while dispatching post-transaction events", exceptionList);
               }
          }
     }
}
