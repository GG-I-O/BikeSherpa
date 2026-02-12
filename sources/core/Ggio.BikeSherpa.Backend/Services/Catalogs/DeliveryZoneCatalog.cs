using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public class DeliveryZoneCatalog : IDeliveryZoneCatalog
{
     public IReadOnlyList<DeliveryZone> DeliveryZones { get; }

     public DeliveryZoneCatalog(IEnumerable<DeliveryZoneEntity> entities)
     {
          DeliveryZones = entities
               .Select(e => new DeliveryZone(
                    e.Name,
                    e.Cities,
                    e.TourPrice,
                    e.Price))
               .ToList();
     }

     public DeliveryZone FromAddress(string city)
     {
          return DeliveryZones.FirstOrDefault(zone => zone.Cities.Contains(city))
          ?? throw new Exception("L’adresse ne se trouve pas dans les zones couvertes.");
     }
}
