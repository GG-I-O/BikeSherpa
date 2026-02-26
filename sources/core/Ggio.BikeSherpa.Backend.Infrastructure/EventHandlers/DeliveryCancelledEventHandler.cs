using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure.EventHandlers;

public class DeliveryCancelledEventHandler(IApplicationTransactionContext context)
     : PostTransactionDomainEventHandlerBase<DeliveryCancelledEvent>(context)
{
     override protected ValueTask HandleInternal(DeliveryCancelledEvent notification, CancellationToken cancellationToken)
     {
          // TODO
          // Send user notification
          return ValueTask.CompletedTask;
     }
}
