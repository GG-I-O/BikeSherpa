using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Clients.GetAll;

public record GetAllClientsQuery: IQuery<List<ClientCrud>>;

public class GetAllClientsHandler(IReadRepository<Client> repository): IQueryHandler<GetAllClientsQuery, List<ClientCrud>>
{
     public async ValueTask<List<ClientCrud>> Handle(GetAllClientsQuery query, CancellationToken ct)
     {
          var allClients = (await repository.ListAsync(ct)).SelectFacets<ClientCrud>();
          return allClients.ToList();
     }
}
