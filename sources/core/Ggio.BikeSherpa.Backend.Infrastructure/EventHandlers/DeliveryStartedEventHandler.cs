using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure.EventHandlers;

public class DeliveryStartedEventHandler(IApplicationTransactionContext context)
     : PostTransactionDomainEventHandlerBase<DeliveryStartedEvent>(context)
{
     override protected ValueTask HandleInternal(DeliveryStartedEvent notification, CancellationToken cancellationToken)
     {
          // TODO
          // Send user notification
          return ValueTask.CompletedTask;
     }
}
