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

     public void ManageSteps(List<DeliveryStep> steps)
     {
          var stepIds = Steps.Select(s => s.Id).ToList();
          
          for (int index = 0; index < steps.Count; index++)
          {
               if (steps[index].Id == Guid.Empty && StepCanFollow(steps[index-1].StepType, steps[index].StepType))
               {
                    AddStep(steps[index].StepType, steps[index].StepAddress, steps[index].StepZone, steps[index].Distance, steps[index].EstimatedDeliveryDate);
               }
               else if (StepCanFollow(steps[index-1].StepType, steps[index].StepType))
               {
                    UpdateStep(steps[index].Id, steps[index].StepType, steps[index].Order, steps[index].StepAddress, steps[index].StepZone, steps[index].Distance, steps[index].EstimatedDeliveryDate);
               }
               else if (!stepIds.Contains(steps[index].Id))
               {
                    DeleteStep(steps[index].Id);
               }
          }
     }
     
     private void AddStep(StepTypeEnum stepTypeEnum, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          var step = new DeliveryStep(stepTypeEnum, Steps.Count + 1, stepAddress, deliveryZone, distance, estimatedDeliveryDate);
          Steps.Add(step);
     }

     private void UpdateStep(Guid id, StepTypeEnum stepType, int order, Address stepAddress, DeliveryZone deliveryZone, double distance, DateTimeOffset estimatedDeliveryDate)
     {
          var existingStep = Steps.Single(s => s.Id == id);
          existingStep.Update(stepType, order, stepAddress, deliveryZone, distance, estimatedDeliveryDate);
     }
     
     private bool StepCanFollow(StepTypeEnum previousStep, StepTypeEnum currentStep) =>
          previousStep == StepTypeEnum.Pickup && currentStep == StepTypeEnum.Dropoff
          || previousStep == StepTypeEnum.Dropoff && currentStep == StepTypeEnum.Dropoff;

     private void DeleteStep(Guid id)
     {
          var step = Steps.Single(s => s.Id == id);
          Steps.Remove(step);
     }

     public double CalculateDeliveryPrice(PackingSize packingSize, Urgency urgencyStrategy)
     {
          double pickupBasePrice = 14;
          double stepPriceInGrenoble;
          double stepPriceInBorder;
          double stepPriceInPeriphery;
          double stepPriceOutside;
          
          switch (PricingStrategy)
          {
               case PricingStrategyEnum.SimpleDeliveryStrategy:
               {
                    return 1;
               }
               case PricingStrategyEnum.CustomStrategy:
               {
                    return 2;
               }
               case PricingStrategyEnum.TourDeliveryStrategy:
               {
                    // stepPriceInGrenoble = DeliveryZoneEnum.Grenoble.TourPrice;
                    // stepPriceInBorder = DeliveryZoneEnum.Border.TourPrice;
                    // stepPriceInPeriphery = DeliveryZoneEnum.Periphery.TourPrice;
                    // stepPriceOutside = DeliveryZoneEnum.Outside.TourPrice;
               }
               default:
               {
                    throw new Exception("Erreur de calcul du prix de livraison.");
               }
          }
     }

     public void AssignSize(PackingSize packingSize)
     {
          PackingSize = packingSize.Name ?? throw new Exception("La taille ne peut pas être nulle.");
     }
}

