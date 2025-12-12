using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.GetAll;

public record GetAllCustomersQuery(DateTimeOffset? lastSync): IQuery<List<CustomerCrud>>;

public class GetAllCustomersHandler(IReadRepository<Customer> repository): IQueryHandler<GetAllCustomersQuery, List<CustomerCrud>>
{
     public async ValueTask<List<CustomerCrud>> Handle(GetAllCustomersQuery query, CancellationToken ct)
     {
          var allCustomers = query.lastSync is null ?
               (await repository.ListAsync(ct)).SelectFacets<CustomerCrud>() :
               (await repository.ListAsync(new ClientByUpdatedAtSpecification((DateTimeOffset)query.lastSync) ,ct)).SelectFacets<CustomerCrud>();
          return allCustomers.ToList();
     }
}
