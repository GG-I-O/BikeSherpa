using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;

public class CourierByCodeSpecification : SingleResultSpecification<Courier>
{
     public CourierByCodeSpecification(string code)
     {
          Query.Where(x => x.Code == code);
     }
}