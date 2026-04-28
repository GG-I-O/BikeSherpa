using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryStepByCourierAndDate: Specification<Delivery>
{
     public DeliveryStepByCourierAndDate(Guid courierId, DateTimeOffset date)
     {
          Query
               .Include(x => x.Steps)
               .Where(x => x.Steps.Any(s => s.EstimatedDeliveryDate.Date == date.Date && s.CourierId == courierId));
     }
}
