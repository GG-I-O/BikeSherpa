using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategy
{
     private readonly double _stepPriceInGrenoble;
     private readonly double _stepPriceInBorder;
     private readonly double _stepPriceInPeriphery;
     private readonly double _stepPriceOutside;

     public SimpleDeliveryStrategy()
     {
          _stepPriceInGrenoble = DeliveryZone.Grenoble.Price;
          _stepPriceInBorder = DeliveryZone.Border.Price;
          _stepPriceInPeriphery = DeliveryZone.Periphery.Price;
          _stepPriceOutside = DeliveryZone.Outside.Price;
     }
     
     public double CalculatePrice(Delivery delivery)
     {
          int stepsInGrenoble = 0;
          int stepsInBorder = 0;
          int stepsInPeriphery = 0;
          int stepsOutside = 0;
          double totalDistance = 0;

          foreach (DeliveryStep step in delivery.Steps)
          {
               totalDistance += step.Distance;
          }
          
          foreach (DeliveryStep step in delivery.Steps)
          {
               switch (step.DropoffZone.Name)
               {
                    case "Grenoble":
                         stepsInGrenoble++;
                         break;
                    case "Limitrophe":
                         stepsInBorder++;
                         break;
                    case "Périphérie":
                         stepsInPeriphery++;
                         break;
                    case "Extérieur":
                         stepsOutside++;
                         break;
               }
          }
          
          return stepsInGrenoble * _stepPriceInGrenoble +
                 stepsInBorder * _stepPriceInBorder +
                 stepsInPeriphery * _stepPriceInPeriphery +
                 stepsOutside * _stepPriceOutside +
                 delivery.Packing.Size.Price +
                 delivery.Urgency.CalculatePrice(totalDistance);
     }

     public int CaclulateOverweightPrice(Delivery delivery)
     {
          int overweightPrice = 0; // TODO
          return overweightPrice;
     }
}
