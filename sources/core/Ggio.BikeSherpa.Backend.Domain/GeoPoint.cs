using System.Globalization;
using System.Text.Json.Serialization;

namespace Ggio.BikeSherpa.Backend.Domain;

public sealed record GeoPoint
{
     private double Longitude { get; }
     private double Latitude { get; }

     [JsonConstructor]
     public GeoPoint(double longitude, double latitude)
     {
          Validate(longitude, latitude);
          Longitude = longitude;
          Latitude = latitude;
     }

     public static GeoPoint FromString(string value)
     {
          if (string.IsNullOrWhiteSpace(value))
               throw new ArgumentException("Imposible de convertir une chaîne de caractères nulle ou vide.", nameof(value));

          var parts = value.Split(',');

          if (parts.Length != 2)
          {
               throw new FormatException($"Format de coordonnées géographiques incorrect : '{value}'. Format attendu : 'longitude,latitude'.");
          }

          if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
          {
               throw new FormatException($"Valeur de longitude invalide : '{parts[0]}'.");
          }

          if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
          {
               throw new FormatException($"valeur de latitude invalide : '{parts[1]}'.");
          }

          return new GeoPoint(lon, lat);
     }

     public override string ToString()
     {
          return $"{Longitude.ToString(CultureInfo.InvariantCulture)},{Latitude.ToString(CultureInfo.InvariantCulture)}";
     }

     private static void Validate(double longitude, double latitude)
     {
          if (longitude is < -180 or > 180)
          {
               throw new ArgumentOutOfRangeException(nameof(longitude), "La longitude doit être comprise entre -180 et 180.");
          }

          if (latitude is < -90 or > 90)
          {
               throw new ArgumentOutOfRangeException(nameof(latitude), "La latitudedoit être comprise entre -90 et 90.");
          }
     }
}
