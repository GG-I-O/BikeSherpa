using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IPricingStrategy
{
     PricingStrategy ImplementedStrategy { get; }

     double CalculateDeliveryPriceWithoutVat(
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
          double extraCost
     );

     double SameDayExtraCost(DateTimeOffset startDate, DateTimeOffset contractDate);
     double DelayCost(DateTimeOffset startDate, DateTimeOffset contractDate);
     double PickupCost(int pickupNumber);
     double DropOffInCoreCost(int dropOffStepsInCore);
     double DropOffInBorderCost(int dropOffStepsInBorder);
     double DropOffInPeripheryCost(int dropOffStepsInPeriphery);
     double DropOffOutsideCost(int dropOffStepsOutside);
     double TotalDistanceCost(double totalDistance, Urgency urgency);
}
