using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService.Contracts;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public class ItineraryService(IItineraryApi itineraryApi, ILogger<ItineraryService> logger) : IItineraryService
{
     private static void EnsureDifferentCoordinates(GeoPoint start, GeoPoint end)
     {
          if (start == end)
          {
               throw ItineraryServiceException.NoIdenticalCoordinates();
          }
     }

     private static void EnsureApiReturnedResult(Itineraire? result)
     {
          if (result is null)
          {
               throw ItineraryServiceException.NoNullResult();
          }
     }

     public async Task<ItineraryResult> GetItineraryInfoAsync(
          GeoPoint startStepCoordinates,
          GeoPoint endStepCoordinates,
          CancellationToken cancellationToken)
     {
          EnsureDifferentCoordinates(startStepCoordinates, endStepCoordinates);

          try
          {
               Console.WriteLine("==========================================================");
               Console.WriteLine("==========================================================");
               Console.WriteLine(startStepCoordinates.ToString());
               Console.WriteLine(endStepCoordinates.ToString());
               Console.WriteLine("==========================================================");
               Console.WriteLine("==========================================================");
               var result = await itineraryApi.RouteItinerairePost(RouteBodyFactory.Create(startStepCoordinates.ToString(), endStepCoordinates.ToString(), [WayType.Highway]), cancellationToken);

               EnsureApiReturnedResult(result);

               return new ItineraryResult(
                    DistanceInKm: result.Distance,
                    TimeInMinutes: result.Duration
               );
          }

          catch (Exception exception)
          {
               logger.LogError(exception, "Error calling the itinerary API");
               
               if (exception is ItineraryServiceException)
               {
                    throw;
               }
               
               throw new ItineraryServiceException("Erreur lors de l’appel à l’API d’itinéraire", exception);
          }
     }
}
