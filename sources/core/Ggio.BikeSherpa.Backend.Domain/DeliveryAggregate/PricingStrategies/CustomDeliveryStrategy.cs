using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public class CustomDeliveryStrategy : IPricingStrategy
{
     public PricingStrategy ImplementedStrategy => PricingStrategy.CustomStrategy;

     public Task<double> CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {
          return Task.FromResult(Math.Round(
               (delivery.TotalPrice ?? 0) +
               (delivery.ExtraCost ?? 0) - (delivery.Discount ?? 0)
               , 2));
     }

     public Task<double> GetStepPrice(Delivery delivery, DeliveryStep deliveryStep)
     {
          return Task.FromResult(0d);
     }
}
