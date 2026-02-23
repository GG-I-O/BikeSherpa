namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategy : IPricingStrategy
{
     private const double PickupBasePrice = 14;
     private const double StepPriceInGrenoble = 5;
     private const double StepPriceInBorder = 8;
     private const double StepPriceInPeriphery = 0;
     private const double StepPriceOutside = 0;

     public string Name => "TourDelivery";

     public double CalculateDeliveryPriceWithoutVat(
          DateTimeOffset startDate,
          DateTimeOffset contractDate,
          int pickupNumber,
          int dropoffStepsInGronoble,
          int dropoffStepsInBorder,
          int dropoffStepsInPeriphery,
          int dropoffStepsOutside,
          PackingSize packingSize,
          double urgencyPriceCoefficient,
          double totalDistance)
     {
          return pickupNumber * PickupBasePrice +
                 PricingRules.CalculateSameDayDeliveryExtraCost(startDate, contractDate) +
                 PricingRules.CalculateDelayCost(startDate, contractDate) +
                 packingSize.TourPrice +
                 dropoffStepsInGronoble * StepPriceInGrenoble +
                 dropoffStepsInBorder * StepPriceInBorder +
                 dropoffStepsInPeriphery * StepPriceInPeriphery +
                 dropoffStepsOutside * StepPriceOutside;
     }
}
