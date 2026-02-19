using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreDeliveryCompletedEventHandler(DbContext dbContext) : INotificationHandler<DeliveryCompletedEvent>
{
     public async ValueTask Handle(DeliveryCompletedEvent notification, CancellationToken cancellationToken)
     {
          await dbContext.AddAsync(notification.NewEntity, cancellationToken);
     }
}
