using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Get;

public record GetClientQuery(Guid Id) : IQuery<CustomerCrud?>;

public class GetClientHandler(IReadRepository<Customer> clientRepository): IQueryHandler<GetClientQuery, CustomerCrud?>
{
     public async ValueTask<CustomerCrud?> Handle(GetClientQuery query, CancellationToken ct)
     {
          var entity = await clientRepository.FirstOrDefaultAsync(new ClientByIdSpecification(query.Id), ct);
          return entity?.ToFacet<CustomerCrud>();
     }
}
