using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public class PackingSizeCatalog : IPackingSizeCatalog
{
     public IReadOnlyList<PackingSize> PackingSizes { get; }

     public PackingSizeCatalog(IEnumerable<PackingSizeEntity> entities)
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

     public PackingSize FromMeasurements(double weight, int length) =>
          PackingSizes.FirstOrDefault(s => weight <= s.MaxWeight && length <= s.MaxLength)
          ?? PackingSizes.Single(s => s.Name == "XXL");
}