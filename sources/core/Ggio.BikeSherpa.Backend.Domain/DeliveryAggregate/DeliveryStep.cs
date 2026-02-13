using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep : EntityBase<Guid>, IAuditEntity
{
     public StepTypeEnum StepType { get; set; }
     public int Order { get; set; }
     public Address StepAddress { get; set; }
     public DeliveryZone StepZone { get; set; }
     public double Distance { get; set; }
     public Guid? CourierId { get; set; }
     public string? Comment { get; set; }
     public string[]? AttachmentFilePaths { get; set; }
     public DateTimeOffset EstimatedDeliveryDate { get; set; }
     public DateTimeOffset? RealDeliveryDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     private DeliveryStep() { }

     public DeliveryStep(StepTypeEnum stepType, int order, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          StepType = stepType;
          Order = order;
          StepAddress = stepAddress;
          StepZone = deliveryZone;
          Distance = distance;
          EstimatedDeliveryDate = estimatedDeliveryDate;
     }

     public void Update(
          StepTypeEnum stepType,
          int order,
          Address stepAddress,
          DeliveryZone deliveryZone,
          double distance,
          DateTimeOffset estimatedDeliveryDate)
     {
          StepType = stepType;
          Order = order;
          StepAddress = stepAddress;
          StepZone = deliveryZone;
          Distance = distance;
          EstimatedDeliveryDate = estimatedDeliveryDate;
     }
}
