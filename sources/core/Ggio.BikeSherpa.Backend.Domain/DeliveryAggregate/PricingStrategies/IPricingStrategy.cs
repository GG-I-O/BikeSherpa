namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IPricingStrategy
{
     double CalculatePrice(Delivery delivery);
}
