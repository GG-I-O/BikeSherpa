namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class PackingSize : Enumeration
{
     public int MaxWeight { get; }
     public int MaxLength { get; }

     public int Price { get; }

     public readonly static PackingSize S = new(1, "S", 3, 45, 3);
     public readonly static PackingSize M = new(2, "M", 10, 85, 5);
     public readonly static PackingSize L = new(3, "L", 20, 105, 7);
     public readonly static PackingSize Xl = new(4, "XL", 30, 115, 9);
     public readonly static PackingSize Xxl = new(5, "XXL", 40, 5000, 11);
     public readonly static PackingSize Xxxl = new(5, "XXXL", 50, 5000, 13);
     public readonly static PackingSize Xxxxl = new(5, "XXXXL", 60, 5000, 15);

     private PackingSize(int id, string name, int maxWeight, int maxLength, int price) : base(id, name)
     {
          MaxWeight = maxWeight;
          MaxLength = maxLength;
          Price = price;
     }

     public static PackingSize? FromMeasurements(double weight, int length)
     {
          return GetAll<PackingSize>()
               .FirstOrDefault(s => weight <= s.MaxWeight && length <= s.MaxLength);
     }
}
