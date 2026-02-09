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
}
