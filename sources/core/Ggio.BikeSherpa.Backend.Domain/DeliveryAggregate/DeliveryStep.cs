using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep : EntityBase<Guid>, IAuditEntity
{
     public StepTypeEnum StepTypeEnum { get; set; }
     public int Order { get; set; }
     public Address StepAddress { get; set; }
     public DeliveryZoneEnum StepZone { get; set; }
     public double Distance { get; set; }
     public Guid? CourierId { get; set; }
     public string? Comment { get; set; }
     public string[]? FilePaths { get; set; }
     public DateTimeOffset EstimatedDeliveryDate { get; set; }
     public DateTimeOffset? RealDeliveryDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     private DeliveryStep() { }

     public DeliveryStep(StepTypeEnum stepTypeEnum, int order, Address stepAddress, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          StepTypeEnum = stepTypeEnum;
          Order = order;
          StepAddress = stepAddress;
          StepZone = DeliveryZoneEnum.FromAddress(stepAddress.City);
          Distance = distance;
          EstimatedDeliveryDate = estimatedDeliveryDate;
     }
}
