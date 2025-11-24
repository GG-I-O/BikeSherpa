namespace Ggio.DddCore;

public abstract class PostTransactionDomainEventHandlerBase<T>(IApplicationTransactionContext context) : IPostTransactionDomainEventHandler<T> where T : IDomainEvent
{
     public async ValueTask Handle(T notification, CancellationToken cancellationToken)
     {
          if (context.Status != IApplicationTransactionContext.TransactionScopeStatus.Completed)
          {
               return;
          }

          await HandleInternal(notification, cancellationToken);
     }

     protected abstract ValueTask HandleInternal(T notification, CancellationToken cancellationToken);
}
