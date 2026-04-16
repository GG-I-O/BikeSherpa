using Mediator;

namespace Ggio.DddCore;

public abstract class FactoryBase(IMediator mediator)
{
     protected async Task NotifyNewAggregateRootAdded<TEntity>(TEntity newEntity) where TEntity : class, IAggregateRoot
     {
          await mediator.Publish(new AggregateRootAddedEvent(newEntity));
     }
}
