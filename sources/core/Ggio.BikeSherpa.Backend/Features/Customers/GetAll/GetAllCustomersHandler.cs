using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.GetAll;

public record GetAllCustomersQuery(DateTimeOffset? lastSync): IQuery<List<Model.CustomerCrud>>;

public class GetAllCustomersHandler(IReadRepository<Customer> repository): IQueryHandler<GetAllCustomersQuery, List<Model.CustomerCrud>>
{
     public async ValueTask<List<Model.CustomerCrud>> Handle(GetAllCustomersQuery query, CancellationToken cancellationToken)
     {
          var allCustomers = query.lastSync is null ?
               (await repository.ListAsync(cancellationToken)).SelectFacets<Customer, Model.CustomerCrud>() :
               (await repository.ListAsync(new CustomerByUpdatedAtSpecification(query.lastSync!.Value) ,cancellationToken)).SelectFacets<Customer, Model.CustomerCrud>();
          return allCustomers.ToList();
     }
}
