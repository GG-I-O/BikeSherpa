using Ardalis.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByCodeSpecification : SingleResultSpecification<Delivery>
{
     public DeliveryByCodeSpecification(string code)
     {
          Query.Where(x => x.Code == code);
     }
}
