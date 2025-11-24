namespace Ggio.DddCore.Infrastructure.ApplicationTransaction;

public class ApplicationTransactionContext : IApplicationTransactionContext
{
     public IEnumerable<IHasDomainEvents> EntitiesWithEvents { get; private set; } = [];
     public IApplicationTransactionContext.TransactionScopeStatus Status { get; private set; } = IApplicationTransactionContext.TransactionScopeStatus.Active;

     public void Complete()
     {
          Status = IApplicationTransactionContext.TransactionScopeStatus.Completed;
     }
}
