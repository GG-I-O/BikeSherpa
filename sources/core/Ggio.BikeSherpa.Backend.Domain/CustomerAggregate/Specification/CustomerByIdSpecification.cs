using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;

public class CustomerByIdSpecification : SingleResultSpecification<Customer>
{
     public CustomerByIdSpecification(Guid id)
     {
          Query.Where(x => x.Id == id);
     }
}
