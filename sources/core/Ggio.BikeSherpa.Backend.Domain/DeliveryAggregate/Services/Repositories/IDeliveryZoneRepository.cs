namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public interface IDeliveryZoneRepository
{
     IReadOnlyList<DeliveryZone> DeliveryZones { get; }
     DeliveryZone FromAddress(string city);
}
