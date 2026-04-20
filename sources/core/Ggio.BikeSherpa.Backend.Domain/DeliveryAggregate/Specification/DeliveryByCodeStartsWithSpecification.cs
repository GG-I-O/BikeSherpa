using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByCodeStartsWithSpecification: Specification<Delivery>
{
     public DeliveryByCodeStartsWithSpecification(string code)
     {
          Query.Where(x => x.Code.StartsWith(code)).OrderBy(x => x.Code);
     }
}
