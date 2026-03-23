using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService.Contracts;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public class ItineraryService(IItineraryApi itineraryApi, ILogger<ItineraryService> logger) : IItinerarySpi
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
               var result = await itineraryApi.RouteItinerairePost(RouteBodyFactory.Create(startStepCoordinates.ToString(), endStepCoordinates.ToString(), [WayType.Highway]), cancellationToken);

               EnsureApiReturnedResult(result);

               return new ItineraryResult(
                    DistanceInKm: result.Distance,
                    TimeInMinutes: result.Duration
               );
          }

          catch (Exception exception) when (exception is not ItineraryServiceException)
          {
               logger.LogError(exception, "Error calling the itinerary API");

               throw new ItineraryServiceException("Erreur lors de l’appel à l’API d’itinéraire", exception);
          }
     }
}
