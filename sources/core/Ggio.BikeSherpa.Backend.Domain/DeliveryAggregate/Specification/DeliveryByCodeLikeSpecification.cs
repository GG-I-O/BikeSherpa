using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByCodeLikeSpecification: Specification<Delivery>
{
     public DeliveryByCodeLikeSpecification(string code)
     {
          Query.Where(x => x.Code.StartsWith(code)).OrderBy(x => x.Code);
     }
}
