using Facet.Mapping;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public class DeliveryStepCrudMapper : IFacetMapConfiguration<DeliveryStepCrud, DeliveryStep>,
     IFacetMapConfiguration<DeliveryStep, DeliveryStepCrud>, IFacetMapConfigurationInstance<DeliveryStep, DeliveryStepCrud>
{
     public static void Map(DeliveryStep source, DeliveryStepCrud target)
     {
          target.PackingSize = source.PackingSize.Name;
     }


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

     void IFacetMapConfigurationInstance<DeliveryStep, DeliveryStepCrud>.Map(DeliveryStep source, DeliveryStepCrud target)
     {
          Map(source, target);
     }

     public static DeliveryStep Map(DeliveryStepCrud source, IPackingSizeRepository packingSizeRepository)
     {
          var target = new DeliveryStep(source.StepType, source.Order, source.StepAddress, source.Comment)
          {
               Id = source.Id,
               Completed = source.Completed,
               StepZone = source.StepZone,
               Distance = source.Distance,
               CourierId = source.CourierId,
               CourierComment = null,
               AttachmentFilePaths = source.AttachmentFilePaths,
               NotBilled = false,
               EstimatedDeliveryDate = source.EstimatedDeliveryDate,
               RealDeliveryDate = source.RealDeliveryDate,
               CreatedAt = source.CreatedAt,
               UpdatedAt = source.UpdatedAt,
               ParentDelivery = null!,
               StepAddress = source.StepAddress,
               PackingSize = packingSizeRepository.GetByName(source.PackingSize) ?? throw new InvalidOperationException("Packing size not found")
          };

          Map(source, target);
          return target;
     }
}
