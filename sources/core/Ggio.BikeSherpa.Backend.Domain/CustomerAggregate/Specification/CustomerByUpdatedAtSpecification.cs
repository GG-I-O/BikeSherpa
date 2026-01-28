using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;

public class CustomerByUpdatedAtSpecification: Specification<Customer>
{
     public CustomerByUpdatedAtSpecification(DateTimeOffset date)
      {
           Query.Where(x => x.UpdatedAt >= date);
      }
}
