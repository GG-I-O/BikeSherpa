using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public class DeliveryStartedEventHandler(IApplicationTransactionContext context)
     : PostTransactionDomainEventHandlerBase<DeliveryStartedEvent>(context)
{
     protected override ValueTask HandleInternal(DeliveryStartedEvent notification, CancellationToken cancellationToken)
     {
          // TODO
          // Send user notification
          return ValueTask.CompletedTask;
     }
}
