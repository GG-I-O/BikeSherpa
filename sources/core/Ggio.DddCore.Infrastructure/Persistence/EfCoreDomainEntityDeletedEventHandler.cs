using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreDomainEntityDeletedEventHandler(DbContext dbContext) : INotificationHandler<DomainEntityDeletedEvent> 
{
     public async ValueTask Handle(DomainEntityDeletedEvent notification, CancellationToken cancellationToken)
     {
          dbContext.Remove(notification.NewEntity);
          await dbContext.SaveChangesAsync(cancellationToken);
     }
}
