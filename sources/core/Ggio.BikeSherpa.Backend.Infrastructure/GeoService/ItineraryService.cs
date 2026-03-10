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
                     Profile = "car",
                     Optimization = "fastest",
                     Constraints =
                     [
                          new Constraint
                         {
                              ConstraintType = ConstraintType.banned,
                              Key = "waytype",
                              Operator = "=",
                              Value = "autoroute"
                         }
                     ],
                     GetSteps = false,
                     GetBbox = false,
                     DistanceUnit = "kilometer",
                     TimeUnit = "minute",
                     Crs = "EPSG:4326"
                },
                cancellationToken
           );

          return new ItineraryResult(
               DistanceInKm: result.Distance,
               TimeInMinutes: result.Duration
          );
     }
}
