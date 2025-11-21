using Mediator;

namespace Ggio.DddCore;

public interface IDomainEventHandler<T> : INotificationHandler<T> where T : IDomainEvent
{
}
