namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class DeliveryZone : Enumeration
{
     private HashSet<string> Cities { get; }
     public double Price { get; }

     public readonly static DeliveryZone Grenoble = new(id: 1, name: "Grenoble", cities: ["Grenoble"], price: 1);

     public readonly static DeliveryZone Limitrophe = new(id: 2, name: "Limitrophe", cities: ["Échirolles", "Eybens", "Fontaine", "La Tronche", "Poisat", "Saint-Martin-d’Hères", "Saint-Martin-le-Vinoux", "Seyssinet-Pariset", "Seyssins"], price: 2.5);

     public readonly static DeliveryZone Periphery = new(id: 3, name: "Périphérie", cities: [], price: 5.5);

     public readonly static DeliveryZone Outside = new(id: 4, name: "Extérieur", cities: [], price: 11);

     private DeliveryZone(int id, string name, IEnumerable<string> cities, double price) : base(id, name)
     {
          Cities = new HashSet<string>(cities, StringComparer.OrdinalIgnoreCase);
          Price = price;
     }

     private bool Matches(string city)
     {
          return !string.IsNullOrWhiteSpace(city) && Cities.Contains(city);
     }

     public static DeliveryZone FromAddress(string city)
     {
          return GetAll<DeliveryZone>()
                      .FirstOrDefault(zone => zone.Matches(city))
                 ?? throw new Exception("L’adresse ne se trouve pas dans les zones couvertes.");
     }
}