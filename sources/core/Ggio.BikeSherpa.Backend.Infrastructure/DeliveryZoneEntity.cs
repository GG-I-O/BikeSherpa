namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class DeliveryZoneEntity
{
     public int Id { get; set; }
     public string Name { get; set; } = null!;
     public HashSet<string> Cities { get; set; } = null!;
     public double TourPrice { get; set; }
     public double Price { get; set; }
}
