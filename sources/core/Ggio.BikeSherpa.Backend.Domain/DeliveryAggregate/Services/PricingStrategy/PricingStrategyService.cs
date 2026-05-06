using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;

public class PricingStrategyService(
     IEnumerable<IPricingStrategy> strategies,
     IUrgencyRepository urgencies,
     IPackingSizeRepository packingSizes
) : IPricingStrategyService
{
     public double CalculateDeliveryPriceWithoutVat(Delivery delivery)
     {
          var strategy = strategies.SingleOrDefault(s => s.ImplementedStrategy == delivery.PricingStrategy);
          if (strategy is null || strategy.ImplementedStrategy == Enumerations.PricingStrategy.CustomStrategy)
          {
               return (double)delivery.TotalPrice!;
          }

          var packingSize = packingSizes.GetByName(delivery.PackingSize);
          if (packingSize is null)
          {
               throw new Exception("Taille de colis invalide");
          }

          var urgencyPriceCoefficient = delivery.Urgency.PriceCoefficient;
          var pickupCount = delivery.Steps.Count(s => s.StepType == StepType.Pickup);
          var dropoffsInCore = delivery.Steps.Count(s => s.StepZone.Name == "Centre");
          var dropoffsInBorder = delivery.Steps.Count(s => s.StepZone.Name == "Limitrophe");
          var dropoffsInPeriphery = delivery.Steps.Count(s => s.StepZone.Name == "Périphérie");
          var dropoffsOutside = delivery.Steps.Count(s => s.StepZone.Name == "Extérieur");

          var totalDistance = delivery.Distance ?? 0;
          
          if (totalDistance == 0)
               totalDistance = delivery.Steps.Where(s => !s.NotBilled).Sum(s => s.Distance);
          
          var price = strategy.CalculateDeliveryPriceWithoutVat(
               delivery.StartDate,
               delivery.ContractDate,
               pickupCount,
               dropoffsInCore,
               dropoffsInBorder,
               dropoffsInPeriphery,
               dropoffsOutside,
               packingSize,
               urgencyPriceCoefficient,
               totalDistance
          );
          
          price -= delivery.Discount ?? 0;
          price += delivery.ExtraCost ?? 0;
          return price;
     }
}
