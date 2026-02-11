namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class PackingSizeEntity
{
     public int Id { get; set; }
     public string Name { get; set; }
     public int MaxWeight { get; set; }
     public int TourMaxLength { get; set; }
     public int MaxLength { get; set; }
     public int TourPrice { get; set; }
     public int Price { get; set; }
}
