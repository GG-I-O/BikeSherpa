using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Services;

public class PackingSizeCatalog
{
     private readonly IReadOnlyList<PackingSize> _sizes;

     public PackingSizeCatalog(IEnumerable<PackingSizeEntity> entities)
     {
          _sizes = entities
               .Select(e => new PackingSize(
                    e.Name,
                    e.MaxWeight,
                    e.TourMaxLength,
                    e.MaxLength,
                    e.TourPrice,
                    e.Price))
               .ToList();
     }

     public IReadOnlyList<PackingSize> All => _sizes;

     public PackingSize FromMeasurements(double weight, int length) =>
          _sizes.FirstOrDefault(s => weight <= s.MaxWeight && length <= s.MaxLength)
          ?? _sizes.Single(s => s.Name == "XXL");
}