namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IPricingStrategy
{
     string Name { get; }

     double CalculateDeliveryPriceWithoutVat(
          DateTimeOffset startDate,
          DateTimeOffset contractDate,
          int pickupNumber,
          int dropoffStepsInGronoble,
          int dropoffStepsInBorder,
          int dropoffStepsInPeriphery,
          int dropoffStepsOutside,
          PackingSize packingSize,
          double urgencyPriceCoefficient,
          double totalDistance
     );
}
