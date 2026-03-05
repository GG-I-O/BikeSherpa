namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record DeliveryZone(string Name)
{
     public ICollection<City> Cities { get; init; } = [];
}
