namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

public interface IDeliveryZoneRepository
{
     IReadOnlyList<DeliveryZone> GetAll();
     DeliveryZone GetByAddress(string city);
}
