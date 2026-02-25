using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.DddCore;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep(StepType stepType, int order, Address stepAddress, DeliveryZone stepZone, double distance, DateTimeOffset estimatedDeliveryDate)
     : EntityBase<Guid>, IAuditEntity
{
     // EF Core requires a parameterless constructor to create an entity instance because it can't create one with complex parameter types like Address and DeliveryZone.
     [UsedImplicitly]
     private DeliveryStep() { }

     public DeliveryStep(StepType stepType, int order, Address stepAddress, double distance)
     {
          StepType = stepType;
          Order = order;
          StepAddress = stepAddress;
          Distance = distance;
     }

     public StepType StepType { get; set; }
     public int Order { get; set; }
     public bool Completed { get; set; }
     public required Address StepAddress { get; set; }
     public required DeliveryZone StepZone { get; set; }
     public double Distance { get; set; }
     public Guid? CourierId { get; set; }
     public string? Comment { get; set; }
     public string[]? AttachmentFilePaths { get; set; }
     public DateTimeOffset EstimatedDeliveryDate { get; set; } = estimatedDeliveryDate;
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