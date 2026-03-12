using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService.Contracts;
using Refit;

namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public class ItineraryService(IItineraryApi itineraryApi) : IItineraryService
{
     private static void EnsureDifferentCoordinates(string start, string end)
     {
          if (start == end)
               throw new ItineraryServiceException("Coordonnées des points de départ et d’arrivée identiques", null);
     }

     private static void EnsureApiReturnedResult(Itineraire? result)
     {
          if (result is null)
               throw new ItineraryServiceException("Aucune réponse de l'API", null);
     }

     public async Task<ItineraryResult> GetItineraryInfoAsync(
          string startStepCoordinates,
          string endStepCoordinates,
          CancellationToken cancellationToken)
     {
          EnsureDifferentCoordinates(startStepCoordinates, endStepCoordinates);

          try
          {
               var result = await itineraryApi.RouteItinerairePost(RouteBodyFactory.Create(startStepCoordinates, endStepCoordinates, [WayType.Autoroute]),
                    cancellationToken
               );

               EnsureApiReturnedResult(result);

               return new ItineraryResult(
                    DistanceInKm: result.Distance,
                    TimeInMinutes: result.Duration
               );
          }
          catch (ApiException)
          {
               throw;
          }

          catch (Exception exception)
          {
               throw new ItineraryServiceException("Erreur lors de l’appel à l’API d’itinéraire", exception);
          }
     }
}
