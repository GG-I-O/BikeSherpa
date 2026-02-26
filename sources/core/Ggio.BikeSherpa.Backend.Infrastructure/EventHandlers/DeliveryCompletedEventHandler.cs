using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure.EventHandlers;

public class DeliveryCompletedEventHandler(IApplicationTransactionContext context)
     : PostTransactionDomainEventHandlerBase<DeliveryCompletedEvent>(context)
{
     override protected ValueTask HandleInternal(DeliveryCompletedEvent notification, CancellationToken cancellationToken)
     {
          // TODO
          // Send user notification
          return ValueTask.CompletedTask;
     }
}
