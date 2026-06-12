using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;

public class PricingStrategyService(
     IEnumerable<IPricingStrategy> strategies
) : IPricingStrategyService
{
     public double CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {
          var strategy = strategies.SingleOrDefault(s => s.ImplementedStrategy == delivery.PricingStrategy);
          if (strategy is null || strategy.ImplementedStrategy == Enumerations.PricingStrategy.CustomStrategy)
          {
               return (double)delivery.TotalPrice!;
          }

          return strategy.CalculateDeliveryPriceWithoutVat(delivery);
     }
}
