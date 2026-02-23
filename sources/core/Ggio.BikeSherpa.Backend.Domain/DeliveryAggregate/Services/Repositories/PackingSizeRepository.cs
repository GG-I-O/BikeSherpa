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
          if (name == "")
          {
               throw new ArgumentException("Veuillez indiquer une taille de colis.");
          }
          else
          {
               return PackingSizes.Single(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase)) ?? throw new ArgumentException("Taille de colis inconnue.");
          }
     }
}
