namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy : IPricingStrategy
{
     private const double StepPriceInGrenoble = 1;
     private const double StepPriceInBorder = 2.5;
     private const double StepPriceInPeriphery = 5.5;
     private const double StepPriceOutside = 11;

     public string Name => "SimpleDelivery";

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

          return PricingRules.CalculateSameDayDeliveryExtraCost(startDate, contractDate) +
                 PricingRules.CalculateDelayCost(startDate, contractDate) +
                 dropoffStepsInGronoble * StepPriceInGrenoble +
                 dropoffStepsInBorder * StepPriceInBorder +
                 dropoffStepsInPeriphery * StepPriceInPeriphery +
                 dropoffStepsOutside * StepPriceOutside +
                 packingSize.Price +
                 urgencyPriceCoefficient * totalDistance;
     }
}
