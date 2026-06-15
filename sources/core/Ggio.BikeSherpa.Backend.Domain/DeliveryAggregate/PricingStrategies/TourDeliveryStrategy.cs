using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategy(IDelayService delayService) : IPricingStrategy
{
     public PricingStrategy ImplementedStrategy => PricingStrategy.TourDeliveryStrategy;

     public async Task<double> CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {
          var delayPrice = (await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Price;

          var pickZonePrice = delivery.Steps.Where(s => s is { StepType: StepType.Pickup, NotBilled: false })
               .Sum(step => step.StepZone.TourPrice);

          var dropSteps = delivery.Steps.Where(s => s is { StepType: StepType.Dropoff, NotBilled: false })
               .ToList();

          var dropStepPrices = dropSteps.Sum(step => step.StepZone.TourPrice);
          var dropStepPackingPrices = dropSteps.Sum(step => step.PackingSize.TourPrice);

          return Math.Round(
               delayPrice +
               pickZonePrice +
               dropStepPackingPrices +
               dropStepPrices +
               (delivery.ExtraCost ?? 0) - (delivery.Discount ?? 0)
               , 2);
     }

     public async Task<double> GetStepPrice(Delivery delivery, DeliveryStep deliveryStep)
     {
          if (deliveryStep.StepType == StepType.Pickup)
          {
               var delayPrice = (await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Price;
               var pickZonePrice = deliveryStep.StepZone.TourPrice;
               return delayPrice + pickZonePrice;
          }
          else // StepType.Dropoff
          {
               var packingPrice = deliveryStep.PackingSize.TourPrice;
               var dropZonePrice = deliveryStep.StepZone.TourPrice;
               return packingPrice + dropZonePrice;
          }
     }
}
