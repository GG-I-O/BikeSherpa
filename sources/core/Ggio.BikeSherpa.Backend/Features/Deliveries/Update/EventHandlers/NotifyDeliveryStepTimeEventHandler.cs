using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update.EventHandlers;

public class NotifyDeliveryStepTimeChangeEventHandler(
     IApplicationTransactionContext context,
     IResourceNotificationService notificationService,
     BackendDbContext dbContext
) : NotifyDeliveryStepEventBase<DeliveryStepTimeChangeEvent>(context, notificationService, dbContext);
