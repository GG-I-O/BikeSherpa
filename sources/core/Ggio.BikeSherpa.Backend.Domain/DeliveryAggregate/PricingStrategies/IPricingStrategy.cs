using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IPricingStrategy
{
     PricingStrategy ImplementedStrategy { get; }

     double CalculateDeliveryPriceWithoutVat(
          DateTimeOffset startDate,
          DateTimeOffset contractDate,
          int pickupNumber,
          int dropoffStepsInCore,
          int dropoffStepsInBorder,
          int dropoffStepsInPeriphery,
          int dropoffStepsOutside,
          PackingSize packingSize,
          double urgencyPriceCoefficient,
          double totalDistance
     );
}
