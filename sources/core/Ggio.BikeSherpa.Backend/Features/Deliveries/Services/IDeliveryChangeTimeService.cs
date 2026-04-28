using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public interface IDeliveryChangeTimeService
{
     Task ChangeTime(Delivery delivery, DeliveryStep step, DateTimeOffset date, CancellationToken cancellationToken);
     Task ChangeOrder(Delivery delivery, DeliveryStep step, int increment, CancellationToken cancellationToken);
}
