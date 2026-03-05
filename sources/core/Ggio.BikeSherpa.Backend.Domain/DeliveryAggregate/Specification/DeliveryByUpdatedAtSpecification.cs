using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByUpdatedAtSpecification : Specification<Delivery>
{
     public DeliveryByUpdatedAtSpecification(DateTimeOffset date)
     {
          Query.Where(x => x.UpdatedAt >= date);
     }
}
