using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;

public class ClientByUpdatedAtSpecification: Specification<Customer>
{
     public ClientByUpdatedAtSpecification(DateTimeOffset date)
      {
           Query.Where(x => x.UpdatedAt >= date);
      }
}
