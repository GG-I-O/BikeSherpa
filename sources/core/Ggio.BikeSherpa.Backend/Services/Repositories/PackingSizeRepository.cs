using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

public class PackingSizeRepository : IPackingSizeRepository
{
     public IReadOnlyList<PackingSize> PackingSizes { get; }

     public PackingSizeRepository(IEnumerable<PackingSizeEntity> entities)
     {
          PackingSizes = entities
               .Select(e => new PackingSize(
                    e.Name,
                    e.MaxWeight,
                    e.MaxPackageLength,
                    e.TourPrice,
                    e.Price))
               .ToList();
     }
     
     public PackingSize FromName(string name)
     {
          return PackingSizes.Single(s => s.Name == name);
     }
}