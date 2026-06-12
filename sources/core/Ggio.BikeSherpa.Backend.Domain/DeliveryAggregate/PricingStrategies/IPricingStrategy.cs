using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IPricingStrategy
{
     PricingStrategy ImplementedStrategy { get; }

     double CalculateDeliveryPriceWithoutVat(
          Delivery delivery
     );

     double DelayCost(DateTimeOffset startDate, DateTimeOffset contractDate);

     double GetStepsInCoreCost(int dropOffStepsInCore);
     double GetStepsInBorderCost(int dropOffStepsInBorder);
     double GetStepsInPeripheryCost(int dropOffStepsInPeriphery);
     double GetOutsideStepsCost(int dropOffStepsOutside);
     double TotalDistanceCost(double totalDistance, Urgency urgency);

     public double GetPackingCost(DeliveryStep deliveryStep);
}
