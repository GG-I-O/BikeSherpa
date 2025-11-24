using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.ApplicationTransaction;

public class ApplicationTransaction<TDbContext>(TDbContext dbContext, IApplicationTransactionContext context, IDomainEventDispatcher domainEventDispatcher) : IApplicationTransaction where TDbContext : DbContext
{
     public async Task CommitAsync(CancellationToken cancellationToken = default)
     {
          if (context.Status != IApplicationTransactionContext.TransactionScopeStatus.Active)
          {
               throw new InvalidOperationException("Cannot dispatch domain events when current application transaction is not active.");
          }

          // Retrieve all tracked entities that have domain events
          var entitiesWithEvents = dbContext.ChangeTracker.Entries<HasDomainEventsBase>()
               .Select(e => e.Entity)
               .Where(e => e.DomainEvents.Any())
               .ToArray();

          // Dispatch domain events
          await domainEventDispatcher.DispatchEventsAsync(entitiesWithEvents);

          // Save changes
          await dbContext.SaveChangesAsync(cancellationToken);

          // Set the current scoped application transaction as completed
          context.Complete();

          //Dispatch events for PostTransactionHandlers
          await domainEventDispatcher.DispatchAndClearEventsAsync(entitiesWithEvents);

     }
}
