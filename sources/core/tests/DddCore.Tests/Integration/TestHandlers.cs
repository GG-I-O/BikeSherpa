using Ggio.DddCore;

namespace DddCore.Tests.Integration;

public class TestTransactionalHandlers(IApplicationTransactionContext context) : TransactionalDomainEventHandlerBase<WonderfulEvent>(context)
{
     public bool WasCalled { get; private set; }

     public uint HandleCount { get; private set; }

     override protected ValueTask HandleInternal(WonderfulEvent notification, CancellationToken cancellationToken)
     {
          WasCalled = true;
          HandleCount++;
          return ValueTask.CompletedTask;
     }
}

public class TestPostTransactionHandlers(IApplicationTransactionContext context) : PostTransactionDomainEventHandlerBase<WonderfulEvent>(context)
{
     public bool WasCalled { get; private set; }

     public uint HandleCount { get; private set; }

     override protected ValueTask HandleInternal(WonderfulEvent notification, CancellationToken cancellationToken)
     {
          WasCalled = true;
          HandleCount++;
          return ValueTask.CompletedTask;
     }
}
