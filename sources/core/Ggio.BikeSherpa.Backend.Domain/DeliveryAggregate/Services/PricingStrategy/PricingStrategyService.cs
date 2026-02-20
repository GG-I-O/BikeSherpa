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
          var strategy = strategies.Single(s => s.Name == delivery.PricingStrategy.ToString());
          var packingSize = packingSizes.FromName(delivery.PackingSize);
          var urgencyPriceCoefficient = urgencies.GetUrgency(delivery.Urgency).PriceCoefficient;
          var pickupCount = delivery.Steps.Count(s => s.StepType == StepTypeEnum.Pickup);
          var dropoffsInGrenoble = delivery.Steps.Count(s => s.StepZone.Name == "Grenoble");
          var dropoffsInBorder = delivery.Steps.Count(s => s.StepZone.Name == "Limitrophe");
          var dropoffsInPeriphery = delivery.Steps.Count(s => s.StepZone.Name == "Périphérie");
          var dropoffsOutside = delivery.Steps.Count(s => s.StepZone.Name == "Extérieur");
          var totalDistance = delivery.Steps.Sum(s => s.Distance);

          if (strategy.Name != "CustomStrategy")
          {
               return strategy.CalculateDeliveryPriceWithoutVat(
                    delivery.StartDate,
                    delivery.ContractDate,
                    pickupCount,
                    dropoffsInGrenoble,
                    dropoffsInBorder,
                    dropoffsInPeriphery,
                    dropoffsOutside,
                    packingSize,
                    urgencyPriceCoefficient,
                    totalDistance
               );               
          }
          else
          {
               return (double)delivery.TotalPrice!;
          }
     }
}
