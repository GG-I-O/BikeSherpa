namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record DeliveryZone(string Name)
{
     public ICollection<City> Cities { get; init; } = [];

     public double TourPrice { get; init; }

     public double SimplePrice { get; init; }
}
