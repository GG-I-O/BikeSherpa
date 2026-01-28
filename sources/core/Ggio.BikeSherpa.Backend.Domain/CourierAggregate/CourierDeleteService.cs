using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate;

public interface ICourierDeleteEventHandler
{
     Task DeleteCourierAsync(Courier courier, CancellationToken cancellationToken);
}

public class CourierDeleteService(IMediator mediator) : DeleteService(mediator), ICourierDeleteEventHandler
{
     public async Task DeleteCourierAsync(Courier courier, CancellationToken cancellationToken = default)
     {
          await NotifyEntityDeleted(courier, cancellationToken);
     }
}