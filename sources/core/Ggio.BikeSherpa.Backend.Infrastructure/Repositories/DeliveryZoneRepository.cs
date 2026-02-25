using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Repositories;

public class DeliveryZoneRepository : IDeliveryZoneRepository
{
     public IReadOnlyList<DeliveryZone> DeliveryZones { get; }

     public DeliveryZoneRepository(IEnumerable<DeliveryZone> entities)
     {
          DeliveryZones = entities
               .Select(e => e with { Cities = e.Cities.Select(c => new City(c.Id, c.Name)).ToList() })
               .ToList();
     }

     public DeliveryZone FromAddress(string city)
     {
          if (city == "")
          {
               throw new ArgumentException("Veuillez indiquer une ville.");
          }
          else
          {
               return DeliveryZones.FirstOrDefault(zone => zone.Cities.Any(c => string.Equals(c.Name, city, StringComparison.CurrentCultureIgnoreCase)))
                      ?? DeliveryZones.First(zone => zone.Name == "Extérieur");
          }
     }
}
