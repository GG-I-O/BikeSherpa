using Ggio.BikeSherpa.Backend.Infrastructure.GeoService.Contracts;

namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public static class RouteBodyFactory
{
     private const string DefaultResource = "bdtopo-osrm";
     private const string DefaultProfile = "car";
     private const string DefaultOptimization = "fastest";
     private const string DefaultDistanceUnit = "kilometer";
     private const string DefaultTimeUnit = "minute";
     private const string DefaultCrs = "EPSG:4326";

     private static Constraint Ban(WayType wayType)
     {
          return new Constraint
          {
               ConstraintType = ConstraintType.Banned,
               Key = "waytype",
               Operator = "=",
               Value = wayType.ToApiValue()
          };
     }

     public static RouteBody Create(string start, string end, List<WayType> wayTypes)
     {
          var constraints = wayTypes.Select(Ban).ToList();

          return new RouteBody
          {
               Resource = DefaultResource,
               Start = start,
               End = end,
               Profile = DefaultProfile,
               Optimization = DefaultOptimization,
               DistanceUnit = DefaultDistanceUnit,
               TimeUnit = DefaultTimeUnit,
               Crs = DefaultCrs,
               GetSteps = false,
               GetBbox = false,
               Constraints = constraints
          };
     }
}
