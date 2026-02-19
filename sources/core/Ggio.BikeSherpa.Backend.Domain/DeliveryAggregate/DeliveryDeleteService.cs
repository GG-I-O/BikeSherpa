using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface IDeliveryDeleteEventHandler
{
     Task DeleteDeliveryAsync(Delivery delivery, CancellationToken cancellationToken);
}

public class DeliveryDeleteService(IMediator mediator) : DeleteService(mediator), IDeliveryDeleteEventHandler
{
     public async Task DeleteDeliveryAsync(Delivery delivery, CancellationToken cancellationToken = default)
     {
          await NotifyEntityDeleted(delivery, cancellationToken);
     }
}
