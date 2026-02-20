namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;

public interface IPricingStrategyService
{
     public double CalculateDeliveryPriceWithoutVat(Delivery delivery);
}
