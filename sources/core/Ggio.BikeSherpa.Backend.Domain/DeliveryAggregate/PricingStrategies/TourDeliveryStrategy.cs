using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategy(IDelayService delayService) : IPricingStrategy
{
     public PricingStrategy ImplementedStrategy => PricingStrategy.TourDeliveryStrategy;

     public double DelayCost(DateTimeOffset startDate, DateTimeOffset contractDate) => delayService.CalculateDelay(startDate, contractDate).Price;


     public double GetStepsInCoreCost(int dropOffStepsInCore) => dropOffStepsInCore * StepPriceInCore;

     public double GetStepsInBorderCost(int dropOffStepsInBorder) => dropOffStepsInBorder * StepPriceInBorder;

     public double GetStepsInPeripheryCost(int dropOffStepsInPeriphery) => dropOffStepsInPeriphery * StepPriceInPeriphery;

     public double GetOutsideStepsCost(int dropOffStepsOutside) => dropOffStepsOutside * StepPriceOutside;

     // public double GetPackingCost(DeliveryStep deliveryStep) => deliveryStep.PackingSize.TourPrice + deliveryStep.StepZone.
     public double TotalDistanceCost(double totalDistance, Urgency urgency) => 0;

     public double CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {

          var delayCost = DelayCost(delivery.StartDate, delivery.ContractDate);

          var pickZonePrice = delivery.Steps.Where(s => s is { StepType: StepType.Pickup, NotBilled: false })
               .Sum(step => step.StepZone.TourPrice);

          var dropSteps = delivery.Steps.Where(s => s is { StepType: StepType.Dropoff, NotBilled: false })
               .ToList();

          var dropStepPrices = dropSteps.Sum(step => step.StepZone.TourPrice);
          var dropStepPackingPrices = dropSteps.Sum(step => step.PackingSize.TourPrice);


          //TODO voir si on garde
          //
          // var inCoreCost = GetStepsInCoreCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.InCore));
          // var inBorderCost = GetStepsInBorderCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.InBorder));
          // var inPeripheryCost = GetStepsInPeripheryCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.InPeriphery));
          // var outsideCost = GetOutsideStepsCost(delivery.Steps.Count(s => s.StepZone.Name == StepZone.Outside));

          return Math.Round(
               delayCost +
               pickZonePrice +
               dropStepPackingPrices +
               dropStepPrices +
               delivery.ExtraCost ?? 0 - delivery.Discount ?? 0
               , 2);
     }
}
