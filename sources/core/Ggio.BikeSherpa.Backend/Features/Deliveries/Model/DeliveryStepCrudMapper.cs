using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public static class DeliveryStepCrudMapper
{
     public static DeliveryStep ToDeliveryStep(
          this DeliveryStepCrud source,
          IDeliveryZoneRepository deliveryZones)
     {
          return new DeliveryStep(
               source.StepType,
               source.Order,
               source.StepAddress,
               source.Comment)
          {
               Id = source.Id,
               Completed = source.Completed,
               StepAddress = source.StepAddress,
               StepZone = deliveryZones.GetByAddress(source.StepAddress.City),
               Distance = source.Distance,
               CourierId = source.CourierId,
               Comment = source.Comment,
               AttachmentFilePaths = source.AttachmentFilePaths,
               EstimatedDeliveryDate = source.EstimatedDeliveryDate,
               RealDeliveryDate = source.RealDeliveryDate,
               CreatedAt = source.CreatedAt,
               UpdatedAt = source.UpdatedAt
          };
     }

     public static List<DeliveryStep> ToDeliverySteps(
          this IEnumerable<DeliveryStepCrud> source,
          IDeliveryZoneRepository deliveryZones)
     {
          return source
               .Select(step => step.ToDeliveryStep(deliveryZones))
               .ToList();
     }
}
