using Ardalis.GuardClauses;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Repositories;

public class PackingSizeRepository(BackendDbContext context) : IPackingSizeRepository
{
     public IReadOnlyList<PackingSize> GetAll()
     {
          return context.PackingSizes.ToList();
     }

     public PackingSize GetByName(string name)
     {
          Guard.Against.NullOrEmpty(name);

          return context.PackingSizes.SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase)) ?? throw new ArgumentException("Taille de colis inconnue.");
     }
}
