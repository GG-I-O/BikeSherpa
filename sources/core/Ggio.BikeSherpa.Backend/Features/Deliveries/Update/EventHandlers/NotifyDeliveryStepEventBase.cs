using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update.EventHandlers;

public abstract class NotifyDeliveryStepEventBase<TEvent>(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService,
     BackendDbContext dbContext
) : PostTransactionDomainEventHandlerBase<TEvent>(context)
     where TEvent : IDeliveryEvent
{
     override protected async ValueTask HandleInternal(TEvent notification, CancellationToken cancellationToken)
     {
          var delivery = await dbContext.Deliveries.FindAsync(notification.DeliveryId, cancellationToken);

          if (delivery is not null)
          {
               await notificationService.NotifyResourceChangeToGroup(
                    "delivery", NotificationOperation.Update, delivery.Id.ToString());
          }
     }
}
