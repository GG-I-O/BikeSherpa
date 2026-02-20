namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategy : IPricingStrategy
{
     private readonly double _pickupBasePrice = 14;
     private readonly double _stepPriceInGrenoble = 5;
     private readonly double _stepPriceInBorder = 8;
     private readonly double _stepPriceInPeriphery = 0;
     private readonly double _stepPriceOutside = 0;
     private const double SameDayDeliveryExtraCost = 2;
     private const double EarlyOrderLimit = 18;
     private const double LastMinuteOrderLimit = 2;
     private const double EarlyOrderDiscount = -2;
     private const double LastMinuteOrderExtraCost = 3;
     private const double StandardCost = 0;
     
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
          return pickupNumber * _pickupBasePrice +
                 CalculateSameDayDeliveryExtraCost(startDate, contractDate) +
                 CalculateDelayCost(startDate, contractDate) +
                 packingSize.TourPrice +
                 dropoffStepsInGronoble * _stepPriceInGrenoble +
                 dropoffStepsInBorder * _stepPriceInBorder +
                 dropoffStepsInPeriphery * _stepPriceInPeriphery +
                 dropoffStepsOutside * _stepPriceOutside;
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
               > EarlyOrderLimit => EarlyOrderDiscount,
               <= LastMinuteOrderLimit => LastMinuteOrderExtraCost,
               _ => StandardCost
          };
     }
}
