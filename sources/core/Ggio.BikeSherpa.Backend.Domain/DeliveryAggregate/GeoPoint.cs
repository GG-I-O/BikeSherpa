using System.Globalization;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record GeoPoint(double Longitude, double Latitude)
{
     public override string ToString()
     {
          return $"{Longitude.ToString(CultureInfo.InvariantCulture)},{Latitude.ToString(CultureInfo.InvariantCulture)}";
     }

     public static GeoPoint TryParse(string input)
     {
          if (string.IsNullOrWhiteSpace(input))
          {
               throw new ArgumentException("Impossible de convertir cette valeur.", nameof(input));
          }

          var parts = input.Split(',');

          if (parts.Length != 2)
          {
               throw new ArgumentException("Coordonnées invalides.", nameof(input));
          }

          var longitude = double.Parse(parts[0], CultureInfo.InvariantCulture);
          var latitude = double.Parse(parts[1], CultureInfo.InvariantCulture);

          return new GeoPoint(longitude, latitude);
     }
}
