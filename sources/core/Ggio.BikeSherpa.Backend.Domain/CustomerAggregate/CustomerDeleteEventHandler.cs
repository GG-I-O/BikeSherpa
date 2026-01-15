using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public interface ICustomerDeleteEventHandler
{
     Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken);
}

public class CustomerDeleteEventHandler(IMediator mediator) : DeleteEventHandler(mediator), ICustomerDeleteEventHandler
{
     public async Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
     {
          await NotifyEntityDeleted(customer, cancellationToken);
     }
}
