using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public interface ICustomerDeleteEventHandler
{
     Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken);
}

public class CustomerDeleteService(IMediator mediator) : DeleteService(mediator), ICustomerDeleteEventHandler
{
     public async Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
     {
          await NotifyEntityDeleted(customer, cancellationToken);
     }
}
