using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategy : IPricingStrategy
{
     private readonly double _pickupBasePrice;
     private readonly double _stepPriceInGrenoble;
     private readonly double _stepPriceInBorder;
     private readonly double _stepPriceInPeriphery;
     private readonly double _stepPriceOutside;

     public TourDeliveryStrategy()
     {
          _pickupBasePrice = 14;
          _stepPriceInGrenoble = DeliveryZone.Grenoble.TourPrice;
          _stepPriceInBorder = DeliveryZone.Border.TourPrice;
          _stepPriceInPeriphery = DeliveryZone.Periphery.TourPrice;
          _stepPriceOutside = DeliveryZone.Outside.TourPrice;
     }

     public double CalculatePrice(Delivery delivery)
     {
          int pickups = 0;
          int dropoffsInGrenoble = 0;
          int dropoffsInBorder = 0;
          int dropoffsInPeriphery = 0;
          int dropoffsOutside = 0;

          foreach (DeliveryStep step in delivery.Steps)
          {
               switch (step.StepType.Name)
               {
                    case "Collecte":
                         pickups++;
                         break;
                    case "Dépôt" when step.StepZone.Name == "Grenoble":
                         dropoffsInGrenoble++;
                         break;
                    case "Dépôt" when step.StepZone.Name == "Limitrophe":
                         dropoffsInBorder++;
                         break;
                    case "Dépôt" when step.StepZone.Name == "Périphérie":
                         dropoffsInPeriphery++;
                         break;
                    case "Dépôt" when step.StepZone.Name == "Extérieur":
                         dropoffsOutside++;
                         break;
               }
          }

          return pickups * _pickupBasePrice +
                 CalculateDelayPrice(delivery) +
                 delivery.Packing.Size.TourPrice +
                 dropoffsInGrenoble * _stepPriceInGrenoble +
                 dropoffsInBorder * _stepPriceInBorder +
                 dropoffsInPeriphery * _stepPriceInPeriphery +
                 dropoffsOutside * _stepPriceOutside;
     }

     private double CalculateDelayPrice(Delivery delivery)
     {
          return delivery.StartDate.Date == delivery.ContractDate.Date ? 2 : 0;
     }
     
     public List<DeliveryStep> AddDeliverySteps(Delivery delivery, Customer customer)
     {
          double pickupNumber = Math.Ceiling(delivery.Weight / 40);
          List<DeliveryStep> pickupSteps = [];

          for (int i = 0; i < pickupNumber; i++)
          {
               DeliveryStep step = new(
                    StepType.Pickup,
                    i+1,
                    customer!.Address,
                    0,
                    delivery.StartDate
               );
          
               pickupSteps.Add(step);

          }
          
          return pickupSteps;
     }
}
