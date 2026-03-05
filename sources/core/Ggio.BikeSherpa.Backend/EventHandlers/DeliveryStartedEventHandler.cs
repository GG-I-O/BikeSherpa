using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.EventHandlers;

public class DeliveryStartedEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService,
     IReadRepository<Delivery> repository)
     : PostTransactionDomainEventHandlerBase<DeliveryStartedEvent>(context)
{
     override protected async ValueTask HandleInternal(DeliveryStartedEvent notification, CancellationToken cancellationToken)
     {
          var delivery = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(notification.DeliveryId), cancellationToken);

          if (delivery is not null)
          {
               foreach (var courierId in delivery.Steps
                             .Where(s => s.CourierId.HasValue)
                             .Select(s => s.CourierId!.Value)
                             .Distinct())
               {
                    await notificationService.NotifyResourceChangeToGroup(
                         "courier", ResourceOperation.Put, courierId.ToString());
               }
          }
     }
}
