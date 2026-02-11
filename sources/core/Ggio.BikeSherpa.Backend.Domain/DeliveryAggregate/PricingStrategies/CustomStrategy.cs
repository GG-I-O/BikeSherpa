using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class CustomStrategy : IPricingStrategy
{
     public CustomStrategy()
     {

     }

     public double CalculatePrice(Delivery delivery)
     {
          return delivery.TotalPrice;
     }

     public List<DeliveryStep> AddDeliverySteps(Delivery delivery, Customer customer)
     {
          double pickupNumber = Math.Ceiling(delivery.TotalWeight / 30);
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
