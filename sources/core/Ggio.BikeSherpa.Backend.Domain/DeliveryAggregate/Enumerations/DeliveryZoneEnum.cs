namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class DeliveryZoneEnum : Enumeration
{
     private HashSet<string> Cities { get; }
     public double TourPrice { get; }
     public double Price { get; }

     public readonly static DeliveryZoneEnum Grenoble = new(
          id: 1,
          name: "Grenoble",
          cities: ["Grenoble"],
          tourPrice: 1,
          price: 5
          );

     public readonly static DeliveryZoneEnum Border = new(
          id: 2,
          name: "Limitrophe",
          cities: ["Échirolles", "Eybens", "Fontaine", "La Tronche", "Poisat", "Saint-Martin-d’Hères", "Saint-Martin-le-Vinoux", "Seyssinet-Pariset", "Seyssins"],
          tourPrice: 2.5,
          price: 8
          );

     public readonly static DeliveryZoneEnum Periphery = new(
          id: 3,
          name: "Périphérie",
          cities: [],
          tourPrice: 5.5,
          price: 0 // Unavailable
          );

     public readonly static DeliveryZoneEnum Outside = new(
          id: 4,
          name: "Extérieur",
          cities: [],
          tourPrice: 11,
          price: 0 // Unavailable
          );

     private DeliveryZoneEnum(int id, string name, IEnumerable<string> cities, double tourPrice, double price) : base(id, name)
     {
          Cities = new HashSet<string>(cities, StringComparer.OrdinalIgnoreCase);
          TourPrice = tourPrice;
          Price = price;
     }

     private bool Matches(string city)
     {
          return !string.IsNullOrWhiteSpace(city) && Cities.Contains(city);
     }

     public static DeliveryZoneEnum FromAddress(string city)
     {
          return GetAll<DeliveryZoneEnum>()
                      .FirstOrDefault(zone => zone.Matches(city))
                 ?? throw new Exception("L’adresse ne se trouve pas dans les zones couvertes.");
     }
}