using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.EventHandlers;

public class DeliveryStartedEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService)
     : PostTransactionDomainEventHandlerBase<DeliveryStartedEvent>(context)
{
     override protected ValueTask HandleInternal(DeliveryStartedEvent notification, CancellationToken cancellationToken)
     {
          notificationService.NotifyResourceChangeToGroup("Course", ResourceOperation.Put, notification.DeliveryId.ToString());
          return ValueTask.CompletedTask;
     }
}
