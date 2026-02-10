using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IPricingStrategy
{
     double CalculatePrice(Delivery delivery);
     List<DeliveryStep> AddDeliverySteps(Delivery delivery, Customer customer);
}
