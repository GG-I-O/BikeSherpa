using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Get;

public record GetClientQuery(Guid Id) : IQuery<CustomerCrud?>;

public class GetCustomerHandler(IReadRepository<Customer> clientRepository): IQueryHandler<GetClientQuery, CustomerCrud?>
{
     public async ValueTask<CustomerCrud?> Handle(GetClientQuery query, CancellationToken ct)
     {
          var entity = await clientRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(query.Id), ct);
          return entity?.ToFacet<CustomerCrud>();
     }
}
