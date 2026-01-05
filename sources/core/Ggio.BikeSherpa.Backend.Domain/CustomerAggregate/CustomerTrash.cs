using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public interface ICustomerTrash
{
     Task DeleteCustomerAsync(Customer customer);
}

public class CustomerTrash(IMediator mediator) : TrashBase(mediator), ICustomerTrash
{
     public async Task DeleteCustomerAsync(Customer customer)
     {
          await NotifyEntityDeleted(customer);
     }
}
