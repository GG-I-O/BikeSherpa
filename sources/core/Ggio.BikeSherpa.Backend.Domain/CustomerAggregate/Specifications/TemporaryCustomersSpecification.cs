using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;

public class TemporaryCustomersSpecification : Specification<Customer>
{
     public TemporaryCustomersSpecification()
     {
          Query.Where(c => c.Code.StartsWith(CustomerFactory.TemporaryCustomerCodePrefix));
     }
     
}
