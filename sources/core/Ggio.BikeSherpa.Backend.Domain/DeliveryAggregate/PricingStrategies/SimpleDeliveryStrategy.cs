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

          return dropoffStepsInGronoble * _stepPriceInGrenoble +
                 dropoffStepsInBorder * _stepPriceInBorder +
                 dropoffStepsInPeriphery * _stepPriceInPeriphery +
                 dropoffStepsOutside * _stepPriceOutside +
                 packingSize.Price +
                 urgencyPriceCoefficient * totalDistance;
     }
}
