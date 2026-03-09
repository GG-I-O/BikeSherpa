using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService.Contracts;

namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public class ItineraryService(IItineraryApi itineraryApi) : IItineraryService
{
     public async Task<ItineraryResult> GetItineraryInfoAsync(
          string startStepCoordinates,
          string endStepCoordinates,
          CancellationToken cancellationToken)
     {
          var result = await itineraryApi.RouteItinerairePost(
                new RouteBody
                {
                     Resource = "bdtopo-osrm",
                     Start = startStepCoordinates,
                     End = endStepCoordinates,
                     Intermediates = [],
                     Profile = "car",
                     Optimization = "fastest",
                     Constraints =
                     [
                          new Constraint
                         {
                              ConstraintType = ConstraintType.Banned,
                              Key = "waytype",
                              Operator = "=",
                              Value = "autoroute"
                         }
                     ],
                     GetSteps = false,
                     GetBbox = false,
                     DistanceUnit = "km",
                     TimeUnit = "min",
                     Crs = "epsg:4326",
                     WaysAttributes = []
                },
                cancellationToken
           );

          return new ItineraryResult(
               DistanceInKm: result.Distance,
               TimeInMinutes: result.Duration
          );
     }
}
