using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;

public class CourierByUpdatedAtSpecification : Specification<Courier>
{
     public CourierByUpdatedAtSpecification(DateTimeOffset date)
     {
          Query.Where(x => x.UpdatedAt >= date);
     }
}