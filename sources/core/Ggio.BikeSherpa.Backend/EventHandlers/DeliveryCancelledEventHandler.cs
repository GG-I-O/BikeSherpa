using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.EventHandlers;

public class DeliveryCancelledEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService)
     : PostTransactionDomainEventHandlerBase<DeliveryCancelledEvent>(context)
{
     override protected ValueTask HandleInternal(DeliveryCancelledEvent notification, CancellationToken cancellationToken)
     {
          notificationService.NotifyResourceChangeToGroup("Course", ResourceOperation.Put, notification.DeliveryId.ToString());
          return ValueTask.CompletedTask;
     }
}
