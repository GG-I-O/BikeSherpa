namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

public interface IDeliveryZoneRepository
{
     IReadOnlyList<DeliveryZone> GetAll();
     DeliveryZone GetByAddress(string city);
}
