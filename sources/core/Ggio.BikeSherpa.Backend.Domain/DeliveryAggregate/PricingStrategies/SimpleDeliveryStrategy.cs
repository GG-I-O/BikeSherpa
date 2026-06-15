using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy(IDelayService delayService) : IPricingStrategy
{
     public PricingStrategy ImplementedStrategy => PricingStrategy.SimpleDeliveryStrategy;

     public async Task<double> CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {
          var delayPrice = (await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Price;
          var dropSteps = delivery.Steps.Where(s => s.StepType == StepType.Dropoff).ToList();
          var pickZonePrice = delivery.Steps.SingleOrDefault(s => s.StepType == StepType.Pickup)?.StepZone.SimplePrice ?? 0;
          var dropZonePrice = dropSteps.Sum(s => s.StepZone.SimplePrice);
          var packingPrice = dropSteps.Sum(s => s.PackingSize.SimplePrice);
          var distancePrice = delivery.Urgency.PriceCoefficient * dropSteps.Sum(s => s.Distance);

          return Math.Round(
               delayPrice +
               pickZonePrice +
               distancePrice +
               dropZonePrice +
               packingPrice +
               (delivery.ExtraCost ?? 0) - (delivery.Discount ?? 0)
               , 2);
     }

     public async Task<double> GetStepPrice(Delivery delivery, DeliveryStep deliveryStep)
     {
          if (deliveryStep.StepType == StepType.Pickup)
          {
               var dropSteps = delivery.Steps.Where(s => s.StepType == StepType.Dropoff).ToList();

               var delayPrice = (await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Price;
               var packingPrice = dropSteps.Sum(s => s.PackingSize.SimplePrice);
               var pickZonePrice = deliveryStep.StepZone.SimplePrice;
               var dropZonePrice = dropSteps.Sum(s => s.StepZone.SimplePrice);

               return delayPrice + packingPrice + pickZonePrice + dropZonePrice;
          }

          // StepType.Dropoff
          return delivery.Urgency.PriceCoefficient * deliveryStep.Distance;
     }
}
