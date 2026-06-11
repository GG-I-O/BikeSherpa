using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;

public class CustomerByCodeSpecification : SingleResultSpecification<Customer>
{
     public CustomerByCodeSpecification(string code)
     {
          Query.Where(x => x.Code == code);
     }
}