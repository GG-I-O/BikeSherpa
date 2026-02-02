using Mediator;

namespace Ggio.DddCore;

public interface ITransactionalDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent;