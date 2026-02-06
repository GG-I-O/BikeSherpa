namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class PackingSize : Enumeration
{
     private int MaxWeight { get; }
     private int TourMaxLength { get; }
     private int MaxLength { get; }
     public int TourPrice { get; }
     public int Price { get; }

     private readonly static PackingSize S = new(
          1,
          "S",
          3,
          45,
          45,
          0,
          3
          );
     private readonly static PackingSize M = new(
          2,
          "M",
          10,
          55,
          85,
          2,
          5
          );
     private readonly static PackingSize L = new(
          3,
          "L",
          20,
          85,
          105,
          4,
          7
          );
     private readonly static PackingSize Xl = new(
          4,
          "XL",
          30,
          105,
          115,
          6,
          9
          );
     private readonly static PackingSize Xxl = new(
          5,
          "XXL",
          40,
          5000,
          5000,
          8,
          11
          );

     private PackingSize(int id, string name, int maxWeight, int tourMaxLength, int maxLength, int tourPrice, int price) : base(id, name)
     {
          MaxWeight = maxWeight;
          TourMaxLength = tourMaxLength;
          MaxLength = maxLength;
          TourPrice = tourPrice;
          Price = price;
     }

     public static PackingSize? FromMeasurements(double weight, int length)
     {
          return GetAll<PackingSize>()
               .FirstOrDefault(size => weight <= size.MaxWeight && length <= size.MaxLength);
     }
}
