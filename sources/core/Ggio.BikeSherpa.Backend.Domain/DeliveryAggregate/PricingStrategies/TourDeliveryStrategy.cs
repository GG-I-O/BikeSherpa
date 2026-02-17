namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategy : IPricingStrategy
{
     private readonly double _pickupBasePrice = 14;
     private readonly double _stepPriceInGrenoble = 5;
     private readonly double _stepPriceInBorder = 8;
     private readonly double _stepPriceInPeriphery = 0;
     private readonly double _stepPriceOutside = 0;
     
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
                 CalculateDelayPrice(startDate, contractDate) +
                 packingSize.TourPrice +
                 dropoffStepsInGronoble * _stepPriceInGrenoble +
                 dropoffStepsInBorder * _stepPriceInBorder +
                 dropoffStepsInPeriphery * _stepPriceInPeriphery +
                 dropoffStepsOutside * _stepPriceOutside;
     }

     // Check if there is an extra cost for a delivery on the same day as the contract
     private static double CalculateDelayPrice(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          return startDate.Date == contractDate.Date ? 2 : 0;
     }
}
