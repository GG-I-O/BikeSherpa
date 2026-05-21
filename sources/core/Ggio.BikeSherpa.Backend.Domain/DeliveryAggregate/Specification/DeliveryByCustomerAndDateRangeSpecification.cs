using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByCustomerAndDateRangeSpecification : Specification<Delivery>
{
     public DeliveryByCustomerAndDateRangeSpecification(
          Guid customerId,
          DateTimeOffset startDate,
          DateTimeOffset endDate
     )
     {
          var utcStart = startDate.ToUniversalTime();
          var utcEnd = endDate.ToUniversalTime();

          Query
               .Where(x => x.CustomerId == customerId &&
                           x.StartDate >= utcStart &&
                           x.StartDate <= utcEnd)
               .OrderBy(x => x.StartDate);
     }
}
