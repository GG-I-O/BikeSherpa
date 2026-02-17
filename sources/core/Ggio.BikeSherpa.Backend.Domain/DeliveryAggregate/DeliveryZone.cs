namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record DeliveryZone(
     int Id,
     string Name,
     IReadOnlyCollection<City> Cities
     );