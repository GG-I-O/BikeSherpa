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
          _stepPriceInGrenoble = DeliveryZoneEnum.Grenoble.TourPrice;
          _stepPriceInBorder = DeliveryZoneEnum.Border.TourPrice;
          _stepPriceInPeriphery = DeliveryZoneEnum.Periphery.TourPrice;
          _stepPriceOutside = DeliveryZoneEnum.Outside.TourPrice;
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
               switch (step.StepTypeEnum.Name)
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
                 delivery.Size.TourPrice +
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
          double pickupNumber = Math.Ceiling(delivery.TotalWeight / 40);
          List<DeliveryStep> pickupSteps = [];

          for (int i = 0; i < pickupNumber; i++)
          {
               DeliveryStep step = new(
                    StepTypeEnum.Pickup,
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
