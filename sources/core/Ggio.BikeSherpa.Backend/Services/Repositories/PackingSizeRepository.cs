using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public class PackingSizeRepository : IPackingSizeRepository
{
     public IReadOnlyList<PackingSize> PackingSizes { get; }

     public PackingSizeRepository(IEnumerable<PackingSizeEntity> entities)
     {
          PackingSizes = entities
               .Select(e => new PackingSize(
                    e.Name,
                    e.MaxWeight,
                    e.TourMaxLength,
                    e.MaxLength,
                    e.TourPrice,
                    e.Price))
               .ToList();
     }

     public PackingSize FromMeasurements(PricingStrategyEnum pricingStrategy, double totalWeight, int highestPakageLength)
     {
          switch (pricingStrategy)
          {
               case PricingStrategyEnum.SimpleDeliveryStrategy:
                    if (highestPakageLength < PackingSizes.Single(s => s.Name == "XXL").MaxLength)
                    {
                         return PackingSizes.FirstOrDefault(s => totalWeight <= s.MaxWeight && highestPakageLength <= s.MaxLength) ?? PackingSizes.Single(s => s.Name == "XXL");
                    }
                    else
                    {
                         throw new Exception("Colis trop volumineux pour une prise en charge à vélo.");
                    }
               case PricingStrategyEnum.TourDeliveryStrategy:
                    if (highestPakageLength < PackingSizes.Single(s => s.Name == "XXL").TourMaxLength)
                    {
                         return PackingSizes.FirstOrDefault(s => totalWeight <= s.MaxWeight && highestPakageLength <= s.TourMaxLength) ?? PackingSizes.Single(s => s.Name == "XXL");
                    }
                    else
                    {
                         throw new Exception("Colis trop volumineux pour une prise en charge à vélo.");
                    }
               default:
                    return PackingSizes.Single(s => s.Name == "XXL");
          }
     }
}