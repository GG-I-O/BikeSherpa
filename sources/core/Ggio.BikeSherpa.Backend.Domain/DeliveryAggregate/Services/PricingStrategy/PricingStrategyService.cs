using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
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

          var pickupCount = delivery.Steps.Count(s => s.StepType == StepType.Pickup);
          var dropOffsInCore = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InCore && s.StepType == StepType.Dropoff);
          var dropOffsInBorder = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InBorder && s.StepType == StepType.Dropoff);
          var dropOffsInPeriphery = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InPeriphery && s.StepType == StepType.Dropoff);
          var dropOffsOutside = delivery.Steps.Count(s => s.StepZone.Name == StepZone.Outside && s.StepType == StepType.Dropoff);

          var totalDistance = delivery.Steps.Where(s => !s.NotBilled).Sum(s => s.Distance);

          var price = strategy.CalculateDeliveryPriceWithoutVat(
               delivery.StartDate,
               delivery.ContractDate,
               pickupCount,
               dropOffsInCore,
               dropOffsInBorder,
               dropOffsInPeriphery,
               dropOffsOutside,
               delivery.Urgency,
               totalDistance,
               delivery.Discount ?? 0,
               delivery.ExtraCost ?? 0
          );

          return price;
     }
}
