namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;

public interface IPricingStrategyService
{
     public Task<double> CalculateDeliveryPriceWithoutVat(Delivery delivery);
}
