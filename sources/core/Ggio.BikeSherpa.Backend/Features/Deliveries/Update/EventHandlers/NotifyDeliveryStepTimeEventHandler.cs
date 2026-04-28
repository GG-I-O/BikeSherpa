using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update.EventHandlers;

public class NotifyDeliveryStepTimeEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService,
     IReadRepository<Delivery> repository
) : NotifyDeliveryStepEventBase<DeliveryStepTimeEvent>(context, notificationService, repository);
