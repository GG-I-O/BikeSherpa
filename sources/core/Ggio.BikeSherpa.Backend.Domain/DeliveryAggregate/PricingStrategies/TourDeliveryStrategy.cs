using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategy : IPricingStrategy
{
     private const double PickupBasePrice = 14;
     private const double StepPriceInCore = 5;
     private const double StepPriceInBorder = 8;
     private const double StepPriceInPeriphery = 0;
     private const double StepPriceOutside = 0;

     public PricingStrategy ImplementedStrategy => PricingStrategy.TourDeliveryStrategy;

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
          return pickupNumber * PickupBasePrice;
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
          return 0;
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
          var pickupCost = PickupCost(pickupNumber);
          var inCoreCost = DropOffInCoreCost(dropOffStepsInCore);
          var inBorderCost = DropOffInBorderCost(dropOffStepsInBorder);
          var inPeripheryCost = DropOffInPeripheryCost(dropOffStepsInPeriphery);
          var outsideCost = DropOffOutsideCost(dropOffStepsOutside);
          
          return Math.Round(
               sameDayCost +
               delayCost +
               pickupCost +
               packingSize.TourPrice +
               inCoreCost +
               inBorderCost +
               inPeripheryCost +
               outsideCost +
               extraCost - discount
               , 2);
     }
}
