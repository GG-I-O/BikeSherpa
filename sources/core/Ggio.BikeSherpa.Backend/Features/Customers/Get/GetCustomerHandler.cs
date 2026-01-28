using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Get;

public record GetCustomerQuery(Guid Id) : IQuery<CustomerCrud?>;

public class GetCustomerHandler(IReadRepository<Customer> customerRepository): IQueryHandler<GetCustomerQuery, CustomerCrud?>
{
     public async ValueTask<CustomerCrud?> Handle(GetCustomerQuery query, CancellationToken ct)
     {
          var entity = await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(query.Id), ct);
          return entity?.ToFacet<CustomerCrud>();
     }
}
