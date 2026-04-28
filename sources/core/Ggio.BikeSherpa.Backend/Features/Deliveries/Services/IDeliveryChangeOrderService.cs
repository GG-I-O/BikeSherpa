using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Services;

public interface IDeliveryChangeOrderService
{
     Task ChangeOrder(Delivery delivery, DeliveryStep step, int increment, CancellationToken cancellationToken);
}
