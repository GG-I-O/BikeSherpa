using Facet.Mapping;
using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public class DeliveryCrudMapper : IFacetMapConfiguration<Delivery, DeliveryCrud>
{
     public static void Map(Delivery source, DeliveryCrud target)
     {
          target.Steps = source.Steps
               .Select(step => new DeliveryStepDto
               {
                    Data = step.ToFacet<DeliveryStep, DeliveryStepCrud>()
               })
               .ToList();
          target.Urgency = source.Urgency.Name;
          target.PackingSize = source.PackingSize.Name;
          target.LimitDate = source.GetLimitDate();
     }
}
