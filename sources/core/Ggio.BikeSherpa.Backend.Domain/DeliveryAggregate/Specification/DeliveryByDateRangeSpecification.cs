using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByDateRangeSpecification : Specification<Delivery>
{
     public DeliveryByDateRangeSpecification(
          DateTimeOffset startDate,
          DateTimeOffset endDate
     )
     {
          var utcStart = startDate.ToUniversalTime();
          var utcEnd = endDate.ToUniversalTime();

          Query
               .Where(x =>
                    x.StartDate >= utcStart &&
                    x.StartDate <= utcEnd)
               .OrderBy(x => x.StartDate);
     }
}
