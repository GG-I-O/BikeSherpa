using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IPricingStrategy
{
     PricingStrategy ImplementedStrategy { get; }

     Task<double> CalculateDeliveryPriceWithoutVat(Delivery delivery);

     Task<double> GetStepPrice(Delivery delivery, DeliveryStep deliveryStep);
}
