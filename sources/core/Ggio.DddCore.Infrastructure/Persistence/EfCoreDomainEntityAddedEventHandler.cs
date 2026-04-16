using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure.Persistence;

public class EfCoreDomainEntityAddedEventHandler(DbContext dbContext) : INotificationHandler<AggregateRootAddedEvent>
{
     public async ValueTask Handle(AggregateRootAddedEvent notification, CancellationToken cancellationToken)
     {
          await dbContext.AddAsync(notification.NewAggregate, cancellationToken);
     }
}
