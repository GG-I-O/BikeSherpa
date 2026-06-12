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

     public double SameDayExtraCost(DateTimeOffset startDate, DateTimeOffset contractDate) => PricingRules.CalculateSameDayDeliveryExtraCost(startDate, contractDate);

     public double DelayCost(DateTimeOffset startDate, DateTimeOffset contractDate) => PricingRules.CalculateDelayCost(startDate, contractDate);

     public double PickupCost(int pickupNumber) => pickupNumber * PickupBasePrice;

     public double DropOffInCoreCost(int dropOffStepsInCore) => dropOffStepsInCore * StepPriceInCore;

     public double DropOffInBorderCost(int dropOffStepsInBorder) => dropOffStepsInBorder * StepPriceInBorder;

     public double DropOffInPeripheryCost(int dropOffStepsInPeriphery) => dropOffStepsInPeriphery * StepPriceInPeriphery;

     public double DropOffOutsideCost(int dropOffStepsOutside) => dropOffStepsOutside * StepPriceOutside;

     public double TotalDistanceCost(double totalDistance, Urgency urgency) => 0;

     public double CalculateDeliveryPriceWithoutVat(DateTimeOffset startDate,
          DateTimeOffset contractDate,
          int pickupNumber,
          int dropOffStepsInCore,
          int dropOffStepsInBorder,
          int dropOffStepsInPeriphery,
          int dropOffStepsOutside,
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
               //TODO  packingSize.TourPrice +
               inCoreCost +
               inBorderCost +
               inPeripheryCost +
               outsideCost +
               extraCost - discount
               , 2);
     }
}
