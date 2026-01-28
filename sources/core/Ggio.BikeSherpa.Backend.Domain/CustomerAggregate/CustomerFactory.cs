using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public interface ICustomerFactory
{
     Task<Customer> CreateCustomerAsync(
          string name,
          string code,
          string? siret,
          string? vatNumber,
          string email,
          string phoneNumber,
          Address address
     );
}

public class CustomerFactory(IMediator mediator) : FactoryBase(mediator), ICustomerFactory
{
     public async Task<Customer> CreateCustomerAsync(string name, string code, string? siret, string? vatNumber, string email, string phoneNumber, Address address)
     {
          var customer = new Customer
          {
               Name = name,
               Code = code,
               Siret = siret,
               VatNumber = vatNumber,
               Email = email,
               PhoneNumber = phoneNumber,
               Address = address
          };

          await NotifyNewEntityAdded(customer);
          return customer;
     }
}