using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.GetAll;

public record GetAllClientsQuery(DateTimeOffset? lastSync): IQuery<List<CustomerCrud>>;

public class GetAllCustomersHandler(IReadRepository<Customer> repository): IQueryHandler<GetAllClientsQuery, List<CustomerCrud>>
{
     public async ValueTask<List<CustomerCrud>> Handle(GetAllClientsQuery query, CancellationToken ct)
     {
          var allClients = query.lastSync is null ?
               (await repository.ListAsync(ct)).SelectFacets<CustomerCrud>() :
               (await repository.ListAsync(new ClientByUpdatedAtSpecification((DateTimeOffset)query.lastSync) ,ct)).SelectFacets<CustomerCrud>();
          return allClients.ToList();
     }
}
