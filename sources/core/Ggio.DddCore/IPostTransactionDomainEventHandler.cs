using Mediator;

namespace Ggio.DddCore;

public interface IPostTransactionDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent;
