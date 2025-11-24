namespace Ggio.DddCore;

public interface IApplicationTransactionContext
{
     public enum TransactionScopeStatus
     {
          Active,
          Completed
     }

     public TransactionScopeStatus Status { get; }

     void Complete();
}
