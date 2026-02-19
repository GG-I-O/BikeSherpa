using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.PricingStrategy;

public interface IPricingStrategyService
{
     public double CalculateDeliveryPriceWithoutVat(Delivery delivery);
}
