using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;

public class DeliveryCancelledEventHandler(IApplicationTransactionContext context)
     : PostTransactionDomainEventHandlerBase<DeliveryCancelledEvent>(context)
{
     protected override ValueTask HandleInternal(DeliveryCancelledEvent notification, CancellationToken cancellationToken)
     {
          // TODO
          // Send user notification
          return ValueTask.CompletedTask;
     }
}
