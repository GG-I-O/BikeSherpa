namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public enum WayType
{
     Highway,
     Tunnel,
     Bridge
}

public static class WayTypeExtensions
{
     public static string ToApiValue(this WayType wayType)
     {
          return wayType switch
          {
               WayType.Highway => "autoroute",
               WayType.Tunnel => "tunnel",
               WayType.Bridge => "pont",
               _ => throw new ArgumentOutOfRangeException(nameof(wayType))
          };
     }
}
