using Facet.Mapping;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public class DeliveryStepCrudMapper : IFacetMapConfiguration<DeliveryStepCrud, DeliveryStep>
{
     public static void Map(DeliveryStepCrud source, DeliveryStep target)
     {
          target.Completed = source.Completed;
          target.Distance = source.Distance;
          target.CourierId = source.CourierId;
          target.AttachmentFilePaths = source.AttachmentFilePaths;
          target.EstimatedDeliveryDate = source.EstimatedDeliveryDate;
          target.RealDeliveryDate = source.RealDeliveryDate;
          target.CreatedAt = source.CreatedAt;
          target.UpdatedAt = source.UpdatedAt;
     }
}
