using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.EventHandlers;

public class NotifyCourierDeliveryCancelledEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService,
     IReadRepository<Delivery> repository)
     : NotifyCourierDeliveryEventBase<DeliveryCancelledEvent>(context, notificationService, repository);
