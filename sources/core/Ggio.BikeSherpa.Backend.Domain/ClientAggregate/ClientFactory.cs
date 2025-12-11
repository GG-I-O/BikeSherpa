using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.ClientAggregate;

public interface IClientFactory
{
     Task<Client> CreateClientAsync(
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
     public async Task<Client> CreateClientAsync(string name, string code, string? siret, string email, string phoneNumber, string address)
     {
          var client = new Client
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
