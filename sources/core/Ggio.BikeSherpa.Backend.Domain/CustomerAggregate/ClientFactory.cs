using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public interface IClientFactory
{
     Task<Customer> CreateClientAsync(
          string name,
          string code,
          string? siret,
          string email,
          string phoneNumber,
          string address
     );
}

public class ClientFactory(IMediator mediator) : FactoryBase(mediator), IClientFactory
{
     public async Task<Customer> CreateClientAsync(string name, string code, string? siret, string email, string phoneNumber, string address)
     {
          var client = new Customer
          {
               Name = name,
               Code = code,
               Siret = siret,
               Email = email,
               PhoneNumber = phoneNumber,
               Address = address
          };

          await NotifyNewEntityAdded(client);
          return client;
     }
}
