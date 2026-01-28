using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;

public class CourierByIdSpecification : SingleResultSpecification<Courier>
{
     public CourierByIdSpecification(Guid id)
     {
          Query.Where(x => x.Id == id);
     }
}