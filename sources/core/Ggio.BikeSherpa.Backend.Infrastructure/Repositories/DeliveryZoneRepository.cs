using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Repositories;

public class DeliveryZoneRepository(BackendDbContext context) : IDeliveryZoneRepository
{
     public IReadOnlyList<DeliveryZone> GetAll()
     {
          return context.DeliveryZones.ToList();
     }

     public DeliveryZone GetByAddress(string city)
     {
          if (city == "")
          {
               throw new ArgumentException("Veuillez indiquer une ville.");
          }
          else
          {
               return context.DeliveryZones
                         .FirstOrDefault(zone => zone.Cities.Any(c => string.Equals(c.Name, city, StringComparison.CurrentCultureIgnoreCase)))
                      ?? context.DeliveryZones.First(zone => zone.Name == "Extérieur");
          }
     }
}
