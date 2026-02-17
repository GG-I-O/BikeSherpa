using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Services.Catalogs;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

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
          return DeliveryZones.FirstOrDefault(zone => zone.Cities.Any(c => c.Name == city))
          ?? throw new Exception("L’adresse ne se trouve pas dans les zones couvertes.");
     }
}
