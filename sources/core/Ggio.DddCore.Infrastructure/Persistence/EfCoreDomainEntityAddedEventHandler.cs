using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreDomainEntityAddedEventHandler(DbContext dbContext) : INotificationHandler<DomainEntityAddedEvent> 
{
     public async ValueTask Handle(DomainEntityAddedEvent notification, CancellationToken cancellationToken)
     {
          await dbContext.AddAsync(notification.NewEntity, cancellationToken);
         // await dbContext.SaveChangesAsync(cancellationToken);
     }
}
