using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy : IPricingStrategy
{
     private const double StepPriceInCore = 1;
     private const double StepPriceInBorder = 2.5;
     private const double StepPriceInPeriphery = 5.5;
     private const double StepPriceOutside = 11;

     public PricingStrategy ImplementedStrategy => PricingStrategy.SimpleDeliveryStrategy;

     public double SameDayExtraCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          return PricingRules.CalculateSameDayDeliveryExtraCost(startDate, contractDate);
     }

     public double DelayCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          return PricingRules.CalculateDelayCost(startDate, contractDate);
     }

     public double PickupCost(int pickupNumber)
     {
          return 0;
     }

     public double DropOffInCoreCost(int dropOffStepsInCore)
     {
          return dropOffStepsInCore * StepPriceInCore;
     }

     public double DropOffInBorderCost(int dropOffStepsInBorder)
     {
          return dropOffStepsInBorder * StepPriceInBorder;
     }

     public double DropOffInPeripheryCost(int dropOffStepsInPeriphery)
     {
          return dropOffStepsInPeriphery * StepPriceInPeriphery;
     }

     public double DropOffOutsideCost(int dropOffStepsOutside)
     {
          return dropOffStepsOutside * StepPriceOutside;
     }

     public double TotalDistanceCost(double totalDistance, Urgency urgency)
     {
          return urgency.PriceCoefficient * totalDistance;
     }

     public double CalculateDeliveryPriceWithoutVat(
          DateTimeOffset startDate,
          DateTimeOffset contractDate,
          int pickupNumber,
          int dropOffStepsInCore,
          int dropOffStepsInBorder,
          int dropOffStepsInPeriphery,
          int dropOffStepsOutside,
          PackingSize packingSize,
          Urgency urgency,
          double totalDistance,
          double discount,
          double extraCost)
     {
          var sameDayCost = SameDayExtraCost(startDate, contractDate);
          var delayCost = DelayCost(startDate, contractDate);
          var inCoreCost = DropOffInCoreCost(dropOffStepsInCore);
          var inBorderCost = DropOffInBorderCost(dropOffStepsInBorder);
          var inPeripheryCost = DropOffInPeripheryCost(dropOffStepsInPeriphery);
          var outsideCost = DropOffOutsideCost(dropOffStepsOutside);
          var distanceCost = TotalDistanceCost(totalDistance, urgency);

          return Math.Round(
               sameDayCost +
               delayCost +
               inCoreCost +
               inBorderCost +
               inPeripheryCost +
               outsideCost +
               packingSize.Price +
               distanceCost +
               extraCost - discount
               , 2);
     }
}
