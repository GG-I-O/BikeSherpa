using Google.Api.Gax.Grpc;
using Google.Maps.Routing.V2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public class GoogleItineraryService(RoutesClient routesClient, ILogger<GoogleItineraryService> logger) : IItinerarySpi
{
     private const string FieldMask = "routes.distanceMeters,routes.duration";

     public async Task<ItineraryResult> GetItineraryInfoAsync(
          GeoPoint startStepCoordinates,
          GeoPoint endStepCoordinates,
          CancellationToken cancellationToken = default)
     {
          EnsureDifferentCoordinates(startStepCoordinates, endStepCoordinates);

          var request = new ComputeRoutesRequest
          {
               Origin = CreateWaypoint(startStepCoordinates),
               Destination = CreateWaypoint(endStepCoordinates),
               TravelMode = RouteTravelMode.Bicycle,
          };

          try
          {
               var callSettings = CallSettings.FromHeader("X-Goog-FieldMask", FieldMask)
                    .WithCancellationToken(cancellationToken);
               var response = await routesClient.ComputeRoutesAsync(request, callSettings);
               var route = response.Routes.FirstOrDefault();

               if (route is null)
               {
                    throw ItineraryServiceException.NoNullResult();
               }

               return new ItineraryResult(
                    Math.Ceiling(route.DistanceMeters / 1000.0),
                    route.Duration.ToTimeSpan().TotalMinutes
               );
          }
          catch (RpcException rpcException)
          {
               logger.LogError(rpcException, "Error calling the Google Routes API");

               throw new ItineraryServiceException("Erreur lors de l’appel à l’API d’itinéraire", rpcException);
          }
          catch (Exception exception) when (exception is not ItineraryServiceException)
          {
               logger.LogError(exception, "Error calling the Google Routes API");

               throw new ItineraryServiceException("Erreur lors de l’appel à l’API d’itinéraire", exception);
          }
     }

     private static Waypoint CreateWaypoint(GeoPoint coordinates)
     {
          return new Waypoint
          {
               Location = new Location
               {
                    LatLng = new Google.Type.LatLng
                    {
                         Latitude = coordinates.Latitude,
                         Longitude = coordinates.Longitude
                    }
               }
          };
     }

     private static void EnsureDifferentCoordinates(GeoPoint start, GeoPoint end)
     {
          if (start == end)
          {
               throw ItineraryServiceException.NoIdenticalCoordinates();
          }
     }
}