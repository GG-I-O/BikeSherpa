using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByCustomerIdSpecification : Specification<Delivery>
{
     public DeliveryByCustomerIdSpecification(Guid customerId)
     {
          Query.Where(x => x.CustomerId == customerId);
     }
}
