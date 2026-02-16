using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Services.Catalogs;

namespace Ggio.BikeSherpa.Backend.Services.Repositories;

public class DeliveryZoneRepository : IDeliveryZoneRepository
{
     public IReadOnlyList<DeliveryZone> DeliveryZones { get; }

     public DeliveryZoneRepository(IEnumerable<DeliveryZoneEntity> entities)
     {
          DeliveryZones = entities
               .Select(e => new DeliveryZone(
                    e.Name,
                    e.Cities.Select(c => new City(c.Name)).ToList()
                    ))
               .ToList();
     }

     public DeliveryZone FromAddress(string city)
     {
          return DeliveryZones.FirstOrDefault(zone => zone.Cities.Any(c => c.Name == city))
          ?? throw new Exception("L’adresse ne se trouve pas dans les zones couvertes.");
     }
}
