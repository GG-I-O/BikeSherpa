using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep : EntityBase<Guid>, IAuditEntity
{
     public StepTypeEnum StepType { get; set; }
     public int Order { get; set; }
     public bool Completed { get; set; } = false;
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
