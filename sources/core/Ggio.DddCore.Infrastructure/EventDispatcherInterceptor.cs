using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ggio.DddCore.Infrastructure;

// Intercepts SaveChanges to dispatch domain events after changes are successfully saved
public class EventDispatchInterceptor<TDbContext>(IDomainEventDispatcher domainEventDispatcher) : SaveChangesInterceptor where TDbContext : DbContext
{
     // Called after SaveChangesAsync has completed successfully
     public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
          CancellationToken cancellationToken = new())
     {
          var context = eventData.Context;
          if (context is not TDbContext appDbContext)
          {
               return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
          }

          // Retrieve all tracked entities that have domain events
          var entitiesWithEvents = appDbContext.ChangeTracker.Entries<HasDomainEventsBase>()
               .Select(e => e.Entity)
               .Where(e => e.DomainEvents.Any())
               .ToArray();

          // Dispatch and clear domain events
          await domainEventDispatcher.DispatchEventsAsync(entitiesWithEvents); //do not clear ^^

          return await base.SavedChangesAsync(eventData, result, cancellationToken);

          //TODO : trouver le moyen de transmettre la liste des entités qui ont des events en cours

     }
}
