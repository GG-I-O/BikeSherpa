using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy : IPricingStrategy
{
     public PricingStrategy ImplementedStrategy => PricingStrategy.SimpleDeliveryStrategy;

     public double DelayCost(DateTimeOffset startDate, DateTimeOffset contractDate) => DelayService.CalculateDelay(startDate, contractDate);

     public double GetStepsInCoreCost(int dropOffStepsInCore) => dropOffStepsInCore * StepPriceInCore;

     public double GetStepsInBorderCost(int dropOffStepsInBorder) => dropOffStepsInBorder * StepPriceInBorder;

     public double GetStepsInPeripheryCost(int dropOffStepsInPeriphery) => dropOffStepsInPeriphery * StepPriceInPeriphery;

     public double GetOutsideStepsCost(int dropOffStepsOutside) => dropOffStepsOutside * StepPriceOutside;

     public double TotalDistanceCost(double totalDistance, Urgency urgency) => urgency.PriceCoefficient * totalDistance;

     public double GetPackingCost(DeliveryStep deliveryStep) => deliveryStep.PackingSize.SimplePrice;

     public double CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {
          var delayPrice = DelayCost(delivery.StartDate, delivery.ContractDate);
          var dropStep = delivery.Steps.Single(s => s.StepType == StepType.Dropoff);
          var pickZonePrice = delivery.Steps.Single(s => s.StepType == StepType.Pickup).StepZone.SimplePrice;
          var dropZonePrice = dropStep.StepZone.SimplePrice;
          var packingPrice = dropStep.PackingSize.SimplePrice;
          var distancePrice = delivery.Urgency.PriceCoefficient * dropStep.Distance;

          //TODO
          // var inCoreCost = GetStepsInCoreCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.InCore));
          // var inBorderCost = GetStepsInBorderCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.InBorder));
          // var inPeripheryCost = GetStepsInPeripheryCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.InPeriphery));
          // var outsideCost = GetOutsideStepsCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.Outside));
          // var distanceCost = TotalDistanceCost(delivery.Steps.Where(s => !s.NotBilled).Sum(s => s.Distance), delivery.Urgency);

          return Math.Round(
               delayPrice +
               pickZonePrice +
               distancePrice +
               dropZonePrice +
               packingPrice +
               delivery.ExtraCost ?? 0 - delivery.Discount ?? 0
               , 2);
     }
}
