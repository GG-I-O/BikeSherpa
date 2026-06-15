using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy(IDelayService delayService) : IPricingStrategy
{
     public PricingStrategy ImplementedStrategy => PricingStrategy.SimpleDeliveryStrategy;

     public async Task<double> CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {
          var delayPrice = (await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Price;
          var dropStep = delivery.Steps.SingleOrDefault(s => s.StepType == StepType.Dropoff);
          var pickZonePrice = delivery.Steps.SingleOrDefault(s => s.StepType == StepType.Pickup)?.StepZone.SimplePrice ?? 0;
          var dropZonePrice = dropStep?.StepZone.SimplePrice ?? 0;
          var packingPrice = dropStep?.PackingSize.SimplePrice ?? 0;
          var distancePrice = delivery.Urgency.PriceCoefficient * dropStep?.Distance ?? 0;

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
               var dropStep = delivery.Steps.SingleOrDefault(s => s.StepType == StepType.Dropoff);
               
               var delayPrice = (await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Price;
               var packingPrice = dropStep?.PackingSize.SimplePrice ?? 0;
               var pickZonePrice = deliveryStep.StepZone.SimplePrice;
               var dropZonePrice = dropStep?.StepZone.SimplePrice ?? 0;
               return delayPrice + packingPrice + pickZonePrice + dropZonePrice;
          }
          else // StepType.Dropoff
          {
               return delivery.Urgency.PriceCoefficient * deliveryStep.Distance;
          }
     }
}
