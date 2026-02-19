using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreDeliveryCancelledEventHandler(DbContext dbContext) : INotificationHandler<DeliveryCancelledEvent>
{
     public async ValueTask Handle(DeliveryCancelledEvent notification, CancellationToken cancellationToken)
     {
          await dbContext.AddAsync(notification.NewEntity, cancellationToken);
     }
}
