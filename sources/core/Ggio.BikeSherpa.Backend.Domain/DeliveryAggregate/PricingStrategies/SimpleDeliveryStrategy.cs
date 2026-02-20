namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy : IPricingStrategy
{
     private const double StepPriceInGrenoble = 1;
     private const double StepPriceInBorder = 2.5;
     private const double StepPriceInPeriphery = 5.5;
     private const double StepPriceOutside = 11;
     private const double SameDayDeliveryExtraCost = 2;
     private const double EarlyOrderLimitInHours = 18;
     private const double LastMinuteOrderLimitInHours = 2;
     private const double EarlyOrderDiscount = -2;
     private const double LastMinuteOrderExtraCost = 3;
     private const double StandardCost = 0;

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

          return CalculateSameDayDeliveryExtraCost(startDate, contractDate) +
                 CalculateDelayCost(startDate, contractDate) +
                 dropoffStepsInGronoble * StepPriceInGrenoble +
                 dropoffStepsInBorder * StepPriceInBorder +
                 dropoffStepsInPeriphery * StepPriceInPeriphery +
                 dropoffStepsOutside * StepPriceOutside +
                 packingSize.Price +
                 urgencyPriceCoefficient * totalDistance;
     }

     // Check if there is an extra cost for a delivery on the same day as the contract
     private static double CalculateSameDayDeliveryExtraCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          return startDate.Date == contractDate.Date ? SameDayDeliveryExtraCost : StandardCost;
     }

     // Check if the delivery delay generates a discount or an extra cost
     private static double CalculateDelayCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          var delayInHours = (contractDate - startDate).TotalHours;
          return delayInHours switch
          {
               > EarlyOrderLimitInHours => EarlyOrderDiscount,
               <= LastMinuteOrderLimitInHours => LastMinuteOrderExtraCost,
               _ => StandardCost
          };
     }
}
