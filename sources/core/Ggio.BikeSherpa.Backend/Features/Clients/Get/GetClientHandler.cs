using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;
using Ggio.BikeSherpa.Backend.Domain.ClientAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Clients.Get;

public record GetClientQuery(Guid Id) : IQuery<ClientCrud?>;

public class GetClientHandler(IReadRepository<Client> clientRepository): IQueryHandler<GetClientQuery, ClientCrud?>
{
     public async ValueTask<ClientCrud?> Handle(GetClientQuery query, CancellationToken ct)
     {
          var entity = await clientRepository.FirstOrDefaultAsync(new ClientByIdSpecification(query.Id), ct);
          return entity?.ToFacet<ClientCrud>();
     }
}
