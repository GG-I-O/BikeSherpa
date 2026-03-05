using Ardalis.GuardClauses;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Repositories;

public class DeliveryZoneRepository(BackendDbContext context) : IDeliveryZoneRepository
{
     public const string FallbackZoneForUnknownCity = "Extérieur";

     public IReadOnlyList<DeliveryZone> GetAll()
     {
          return context.DeliveryZones.ToList();
     }

     public DeliveryZone GetByAddress(string city)
     {
          Guard.Against.NullOrEmpty(city);

          return context.DeliveryZones.FirstOrDefault(zone => zone.Cities.Any(c => string.Equals(c.Name, city, StringComparison.InvariantCultureIgnoreCase))) ?? context.DeliveryZones.First(zone => zone.Name == FallbackZoneForUnknownCity);
     }
}
