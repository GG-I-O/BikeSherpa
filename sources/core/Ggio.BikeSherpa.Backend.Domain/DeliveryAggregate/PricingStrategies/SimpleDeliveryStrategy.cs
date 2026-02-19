namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy : IPricingStrategy
{
     private readonly double _stepPriceInGrenoble = 1;
     private readonly double _stepPriceInBorder = 2.5;
     private readonly double _stepPriceInPeriphery = 5.5;
     private readonly double _stepPriceOutside = 11;
     
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
                 dropoffStepsInGronoble * _stepPriceInGrenoble +
                 dropoffStepsInBorder * _stepPriceInBorder +
                 dropoffStepsInPeriphery * _stepPriceInPeriphery +
                 dropoffStepsOutside * _stepPriceOutside +
                 packingSize.Price +
                 urgencyPriceCoefficient * totalDistance;
     }
     
     // Check if there is an extra cost for a delivery on the same day as the contract
     private static double CalculateSameDayDeliveryExtraCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          return startDate.Date == contractDate.Date ? 2 : 0;
     }
     
     // Check if the delivery delay generates a discount or an extra cost
     private static double CalculateDelayCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          var delayInHours = (contractDate - startDate).TotalHours;
          return delayInHours switch
          {
               > 18 => -2,
               <= 2 => 3,
               _ => 0
          };
     }
}
