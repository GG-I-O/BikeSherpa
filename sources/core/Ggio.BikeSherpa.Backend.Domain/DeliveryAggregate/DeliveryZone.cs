namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record DeliveryZone(
     string Name,
     IReadOnlyCollection<City> Cities
     );