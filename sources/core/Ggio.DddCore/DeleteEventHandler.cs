using Mediator;

namespace Ggio.DddCore;

public abstract class DeleteEventHandler(IMediator mediator)
{
     protected async Task NotifyEntityDeleted<TEntity>(TEntity deletedEntity, CancellationToken cancellationToken) where TEntity : class, IAggregateRoot
     {
          await mediator.Publish(new DomainEntityDeletedEvent(deletedEntity), cancellationToken);
     }
}