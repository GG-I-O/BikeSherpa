using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;

public class CourierByEmailSpecification : SingleResultSpecification<Courier>
{
     public CourierByEmailSpecification(string email)
     {
          Query.Where(x => x.Email == email);
     }
}
