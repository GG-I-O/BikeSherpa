using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreDeliveryStartedEventHandler(DbContext dbContext) : INotificationHandler<DeliveryStartedEvent>
{
     public async ValueTask Handle(DeliveryStartedEvent notification, CancellationToken cancellationToken)
     {
          await dbContext.AddAsync(notification.NewEntity, cancellationToken);
     }
}
