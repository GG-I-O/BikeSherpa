using Mediator;

namespace Ggio.DddCore;

public abstract class TrashBase(IMediator mediator)
{
     protected async Task NotifyEntityDeleted<TEntity>(TEntity deletedEntity) where TEntity : class, IAggregateRoot
     {
          await mediator.Publish(new DomainEntityDeletedEvent(deletedEntity));
     }
}