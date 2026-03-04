using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep : EntityBase<Guid>, IAuditEntity
{
     private DeliveryStep() { }

     public DeliveryStep(StepType stepType, int order, Address stepAddress, DeliveryZone stepZone, double distance)
     {
          StepType = stepType;
          Order = order;
          StepAddress = stepAddress;
          StepZone = stepZone;
          Distance = distance;
     }

     public StepType StepType { get; set; }
     public int Order { get; set; }
     public bool Completed { get; set; }
     public Address StepAddress { get; set; } = null!;
     public DeliveryZone StepZone { get; set; } = null!;
     public double Distance { get; set; }
     public Guid? CourierId { get; set; }
     public string? Comment { get; set; }
     public string[]? AttachmentFilePaths { get; set; }
     public DateTimeOffset EstimatedDeliveryDate { get; set; }
     public DateTimeOffset? RealDeliveryDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     public void Update(
          StepType stepType,
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
}