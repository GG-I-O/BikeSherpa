using Ardalis.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByIdSpecification : SingleResultSpecification<Delivery>
{
     public DeliveryByIdSpecification(Guid id)
     {
          Query.Where(x => x.Id == id);
     }
}
