using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep : EntityBase<Guid>, IAuditEntity
{
     public required StepType StepType { get; set; }
     public required int Order { get; set; }
     public required Address DropoffAddress { get; set; }
     public required DeliveryZone DropoffZone { get; set; }
     public required double Distance { get; set; }
     public Guid? CourierId { get; set; }
     public string? Comment { get; set; }
     public string[]? FilePaths { get; set; }
     public required DateTimeOffset EstimatedDeliveryDate { get; set; }
     public DateTimeOffset? RealDeliveryDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     private DeliveryStep() { }

     public DeliveryStep(StepType stepType, int order, Address dropoffAddress, double distance, Urgency urgency, double price, DateTimeOffset contractDate, DateTimeOffset estimatedDeliveryDate)
     {
          StepType = stepType;
          Order = order;
          DropoffAddress = dropoffAddress;
          DropoffZone = DeliveryZone.FromAddress(dropoffAddress.City);
          Distance = distance;
          EstimatedDeliveryDate = estimatedDeliveryDate;
     }
}
