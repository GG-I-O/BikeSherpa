namespace Ggio.DddCore;

public abstract class TransactionalDomainEventHandlerBase<T>(IApplicationTransactionContext context) : ITransactionalDomainEventHandler<T> where T : IDomainEvent
{
     public async ValueTask Handle(T notification, CancellationToken cancellationToken)
     {
          if (context.Status != IApplicationTransactionContext.TransactionScopeStatus.Active)
          {
               return;
          }

          await HandleInternal(notification, cancellationToken);
     }

     protected abstract ValueTask HandleInternal(T notification, CancellationToken cancellationToken);
}
