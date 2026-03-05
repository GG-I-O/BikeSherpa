using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.EventHandlers;

public class NotifyCourierDeliveryStartedEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService,
     IReadRepository<Delivery> repository)
     : NotifyCourierDeliveryEventBase<DeliveryStartedEvent>(context, notificationService, repository);
