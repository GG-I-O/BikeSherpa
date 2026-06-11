using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;

public class CustomerByUpdatedAtSpecification: Specification<Customer>
{
     public CustomerByUpdatedAtSpecification(DateTimeOffset date)
      {
           Query.Where(x => x.UpdatedAt >= date);
      }
}
