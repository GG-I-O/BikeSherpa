using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;

public class ClientByIdSpecification : SingleResultSpecification<Customer>
{
     public ClientByIdSpecification(Guid id)
     {
          Query.Where(x => x.Id == id);
     }
}
