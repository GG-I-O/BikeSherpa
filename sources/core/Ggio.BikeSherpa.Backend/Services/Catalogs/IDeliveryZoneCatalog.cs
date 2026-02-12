using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

namespace Ggio.BikeSherpa.Backend.Services.Catalogs;

public interface IDeliveryZoneCatalog
{
     IReadOnlyList<DeliveryZone> DeliveryZones { get; }
     DeliveryZone FromAddress(string city);
}
