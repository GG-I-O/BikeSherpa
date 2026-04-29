using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public interface IDeliveryChangeTimeService
{
     Task ChangeTime(DeliveryStep step, DateTimeOffset date, CancellationToken cancellationToken);
     Task ChangeOrder(DeliveryStep step, MoveDirection moveDirection, CancellationToken cancellationToken);
}
