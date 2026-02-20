namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public class PackingSizeRepository : IPackingSizeRepository
{
     public IReadOnlyList<PackingSize> PackingSizes { get; }

     public PackingSizeRepository(IEnumerable<PackingSize> entities)
     {
          PackingSizes = entities
               .Select(e => new PackingSize(
                    e.Id,
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
