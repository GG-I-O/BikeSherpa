using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByCourierAndDateRangeSpecification : Specification<Delivery>
{
     public DeliveryByCourierAndDateRangeSpecification(
          Guid courierId,
          DateTimeOffset startDate,
          DateTimeOffset endDate
     )
     {
          var utcStart = startDate.ToUniversalTime();
          var utcEnd = endDate.ToUniversalTime();

          Query
               .Where(x => x.Steps.Any(s => s.CourierId == courierId) &&
                           x.StartDate >= utcStart &&
                           x.StartDate <= utcEnd)
               .OrderBy(x => x.StartDate);
     }
}
