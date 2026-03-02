using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.EventHandlers;

public class DeliveryCompletedEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService)
     : PostTransactionDomainEventHandlerBase<DeliveryCompletedEvent>(context)
{
     override protected ValueTask HandleInternal(DeliveryCompletedEvent notification, CancellationToken cancellationToken)
     {
          notificationService.NotifyResourceChangeToGroup("Course", ResourceOperation.Put, notification.DeliveryId.ToString());
          return ValueTask.CompletedTask;
     }
}
