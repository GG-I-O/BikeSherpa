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
          if (strategy is null)
          {
               return (double)delivery.TotalPrice!;
          }

          var packingSize = packingSizes.GetByName(delivery.PackingSize);
          if (packingSize is null)
          {
               throw new Exception("Taille de colis invalide");
          }

          var urgency = urgencies.GetByName(delivery.Urgency);
          if (urgency is null)
          {
               throw new Exception("Urgence invalide");
          }

          var urgencyPriceCoefficient = urgency.PriceCoefficient;
          var pickupCount = delivery.Steps.Count(s => s.StepType == StepType.Pickup);
          var dropoffsInCore = delivery.Steps.Count(s => s.StepZone.Name == "Centre");
          var dropoffsInBorder = delivery.Steps.Count(s => s.StepZone.Name == "Limitrophe");
          var dropoffsInPeriphery = delivery.Steps.Count(s => s.StepZone.Name == "Périphérie");
          var dropoffsOutside = delivery.Steps.Count(s => s.StepZone.Name == "Extérieur");
          var totalDistance = delivery.Steps.Sum(s => s.Distance);
          
          return strategy.CalculateDeliveryPriceWithoutVat(
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
     }
}
