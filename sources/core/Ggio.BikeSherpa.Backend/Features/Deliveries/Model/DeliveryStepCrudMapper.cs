using Facet.Mapping;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public class DeliveryStepCrudMapper : IFacetMapConfiguration<DeliveryStep, DeliveryStepCrud>
{
     public static void Map(DeliveryStep source, DeliveryStepCrud target)
     {
          
     }

     public static DeliveryStep ToDomain(DeliveryStepCrud source, Delivery parentDelivery)
     {
          return new DeliveryStep(
               source.StepType,
               parentDelivery.Steps.Count + 1,
               source.StepAddress,
               source.Comment)
          {
               Id = source.Id,
               StepAddress = source.StepAddress,
               StepZone = source.StepZone,
               ParentDelivery = parentDelivery,
               Completed = source.Completed,
               Distance = source.Distance,
               CourierId = source.CourierId,
               AttachmentFilePaths = source.AttachmentFilePaths,
               EstimatedDeliveryDate = source.EstimatedDeliveryDate,
               RealDeliveryDate = source.RealDeliveryDate,
               CreatedAt = source.CreatedAt,
               UpdatedAt = source.UpdatedAt
          };
     }
}
