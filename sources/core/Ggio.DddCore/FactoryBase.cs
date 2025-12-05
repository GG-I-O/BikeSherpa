using Mediator;

namespace Ggio.DddCore;

public abstract class FactoryBase(IMediator mediator)
{
     protected async Task NotifyNewEntityAdded<TEntity>(TEntity newEntity) where TEntity : class, IAggregateRoot
     {
          await mediator.Publish(new DomainEntityAddedEvent(newEntity));
     }
}
