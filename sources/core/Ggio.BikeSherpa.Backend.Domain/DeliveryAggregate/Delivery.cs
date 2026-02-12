using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required PricingStrategyEnum PricingStrategyEnum { get; set; }
     public required DeliveryStatusEnum StatusEnum { get; set; }
     public required string Code { get; set; }
     public required Guid CustomerId { get; set; }
     public UrgencyEnum? Urgency { get; set; }
     public required double TotalPrice { get; set; }
     public required Guid ReportId { get; set; }
     public List<DeliveryStep> Steps { get; set; } = [];
     public required string[] Details { get; set; }
     public required double TotalWeight { get; set; }
     public required int HighestPackageLength { get; set; }
     public required PackingSize Size { get; set; }
     public required DateTimeOffset ContractDate { get; set; }
     public required DateTimeOffset StartDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     public DeliveryStep AddStep(StepTypeEnum stepTypeEnum, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          var step = new DeliveryStep(stepTypeEnum, Steps.Count + 1, stepAddress, deliveryZone, distance, estimatedDeliveryDate);
          Steps.Add(step);
          return step;
     }

     public void DeleteStep(DeliveryStep step)
     {
          Steps.Remove(step);
     }

     public double CalculateDeliveryPrice()
     {
          return 0;
     }
}

