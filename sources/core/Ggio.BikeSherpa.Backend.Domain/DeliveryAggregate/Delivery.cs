using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required PricingStrategyEnum PricingStrategy { get; set; }
     public DeliveryStatusEnum? Status { get; set; }
     public required string Code { get; set; }
     public required Guid CustomerId { get; set; }
     public required string Urgency { get; set; }
     public double TotalPrice { get; set; }
     public required Guid ReportId { get; set; }
     public List<DeliveryStep> Steps { get; set; } = [];
     public string[]? Details { get; set; }
     public required double TotalWeight { get; set; }
     public required int HighestPackageLength { get; set; }
     public string? PackingSize { get; set; }
     public required DateTimeOffset ContractDate { get; set; }
     public required DateTimeOffset StartDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     public void AddStep(StepTypeEnum stepTypeEnum, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          var step = new DeliveryStep(stepTypeEnum, Steps.Count + 1, stepAddress, deliveryZone, distance, estimatedDeliveryDate);
          Steps.Add(step);
     }

     public void UpdateStep(Guid id, StepTypeEnum stepType, int order, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          var existingStep = Steps.Single(s => s.Id == id);
          existingStep.Update(stepType, order, stepAddress, deliveryZone, distance, estimatedDeliveryDate);
     }

     public void DeleteStep(Guid id)
     {
          var step = Steps.Single(s => s.Id == id);
          Steps.Remove(step);
     }

     public double CalculateDeliveryPrice(PackingSize packingSize, Urgency urgencyStrategy)
     {
          switch (PricingStrategy)
          {
               case PricingStrategyEnum.SimpleDeliveryStrategy:
                    return 1;
               case PricingStrategyEnum.CustomStrategy:
                    return 2;
               case PricingStrategyEnum.TourDeliveryStrategy:
                    return 3;
               default:
                    throw new Exception("Erreur de calcul du prix de livraison.");
          }
     }

     public void AssignSize(PackingSize packingSize)
     {
          PackingSize = packingSize.Name ?? throw new Exception("La taille ne peut pas être nulle.");
     }
}

