using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreDomainEntityDeletedEventHandler(DbContext dbContext) : INotificationHandler<AggregateRootDeletedEvent>
{
     public async ValueTask Handle(AggregateRootDeletedEvent notification, CancellationToken cancellationToken)
     {
          dbContext.Remove(notification.DeletedAggregate);
          await dbContext.SaveChangesAsync(cancellationToken);
     }
}
