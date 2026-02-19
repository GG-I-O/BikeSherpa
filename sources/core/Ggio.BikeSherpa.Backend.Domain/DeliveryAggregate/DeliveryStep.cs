using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep(StepTypeEnum stepType, int order, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     : EntityBase<Guid>, IAuditEntity
{
     public StepTypeEnum StepType { get; set; } = stepType;
     public int Order { get; set; } = order;
     public bool Completed { get; set; }
     public Address StepAddress { get; set; } = stepAddress;
     public DeliveryZone StepZone { get; set; } = deliveryZone;
     public double Distance { get; set; } = distance;
     public Guid? CourierId { get; set; }
     public string? Comment { get; set; }
     public string[]? AttachmentFilePaths { get; set; }
     public DateTimeOffset EstimatedDeliveryDate { get; set; } = estimatedDeliveryDate;
     public DateTimeOffset? RealDeliveryDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     public void Update(
          StepTypeEnum stepType,
          int order,
          bool completed,
          Address stepAddress,
          DeliveryZone deliveryZone,
          double distance,
          DateTimeOffset estimatedDeliveryDate)
     {
          StepType = stepType;
          Order = order;
          Completed = completed;
          StepAddress = stepAddress;
          StepZone = deliveryZone;
          Distance = distance;
          EstimatedDeliveryDate = estimatedDeliveryDate;
     }
     
     public void UpdateOrder(int order) { Order = order; }
     
     public void AssignCourier(Guid courierId) { CourierId = courierId; }
     
     public void UpdateDeliveryTime(DateTimeOffset deliveryTime) { EstimatedDeliveryDate = deliveryTime; }
     
     public void UpdateCompletion(bool completed) { Completed = completed; }
}