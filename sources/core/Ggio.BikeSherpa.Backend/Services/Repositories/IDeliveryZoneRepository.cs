using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public interface IDeliveryZoneRepository
{
     IReadOnlyList<DeliveryZone> DeliveryZones { get; }
     DeliveryZone FromAddress(string city);
}
