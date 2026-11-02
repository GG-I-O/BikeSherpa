using AwesomeAssertions;
using Google.Api.Gax.Grpc;
using Google.Maps.Routing.V2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Infrastructure.Tests;

public class GoogleItineraryServiceTests
{
     private readonly Mock<RoutesClient> _routesClient = new();
     private readonly Mock<ILogger<GoogleItineraryService>> _logger = new();

     [Fact]
     public async Task GetItineraryInfoAsync_WithValidCoordinates_ReturnsRoundedDistanceAndDuration()
     {
          _routesClient.Setup(client => client.ComputeRoutesAsync(
                    It.IsAny<ComputeRoutesRequest>(),
                    It.IsAny<CallSettings>()))
               .ReturnsAsync(new ComputeRoutesResponse
               {
                    Routes =
                    {
                         new Route
                         {
                              DistanceMeters = 25_250,
                              Duration = Duration.FromTimeSpan(TimeSpan.FromMinutes(30.5))
                         }
                    }
               });

          var service = new GoogleItineraryService(_routesClient.Object, _logger.Object);

          var result = await service.GetItineraryInfoAsync(
               new GeoPoint(2.35, 48.85), new GeoPoint(2.45, 48.95), CancellationToken.None);

          result.DistanceInKm.Should().Be(26);
          result.TimeInMinutes.Should().Be(30.5);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_SendsWaypointsAndAvoidHighwaysModifier()
     {
          _routesClient.Setup(client => client.ComputeRoutesAsync(
                    It.IsAny<ComputeRoutesRequest>(),
                    It.IsAny<CallSettings>()))
               .ReturnsAsync(new ComputeRoutesResponse { Routes = { new Route
                         {
                              Duration = new Duration { Seconds = 1830 },
                              DistanceMeters = 26000
                         } } });

          var service = new GoogleItineraryService(_routesClient.Object, _logger.Object);

          await service.GetItineraryInfoAsync(
               new GeoPoint(2.35, 48.85), new GeoPoint(2.45, 48.95), CancellationToken.None);

          _routesClient.Verify(client => client.ComputeRoutesAsync(
               It.Is<ComputeRoutesRequest>(request =>
                    request.Origin.Location.LatLng.Longitude == 2.35 &&
                    request.Origin.Location.LatLng.Latitude == 48.85 &&
                    request.Destination.Location.LatLng.Longitude == 2.45 &&
                    request.Destination.Location.LatLng.Latitude == 48.95 &&
                    request.TravelMode == RouteTravelMode.Bicycle),
               It.IsAny<CallSettings>()), Times.Once);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_ThrowsWhenCoordinatesAreIdentical()
     {
          var service = new GoogleItineraryService(_routesClient.Object, _logger.Object);

          var exception = await Assert.ThrowsAsync<ItineraryServiceException>(() =>
               service.GetItineraryInfoAsync(new GeoPoint(2.35, 48.85), new GeoPoint(2.35, 48.85), CancellationToken.None));

          exception.Message.Should().Be("Coordonnées de départ et d’arrivée identiques");
          _routesClient.Verify(client => client.ComputeRoutesAsync(
               It.IsAny<ComputeRoutesRequest>(), It.IsAny<CallSettings>()), Times.Never);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_ThrowsWhenApiReturnsNoRoutes()
     {
          _routesClient.Setup(client => client.ComputeRoutesAsync(
                    It.IsAny<ComputeRoutesRequest>(),
                    It.IsAny<CallSettings>()))
               .ReturnsAsync(new ComputeRoutesResponse());

          var service = new GoogleItineraryService(_routesClient.Object, _logger.Object);

          var exception = await Assert.ThrowsAsync<ItineraryServiceException>(() =>
               service.GetItineraryInfoAsync(new GeoPoint(2.35, 48.85), new GeoPoint(2.45, 48.95), CancellationToken.None));

          exception.Message.Should().Be("Aucune réponse de l'API");
     }

     [Fact]
     public async Task GetItineraryInfoAsync_WrapsRpcException()
     {
          var rpcException = new RpcException(new Status(StatusCode.Unavailable, "unavailable"));
          _routesClient.Setup(client => client.ComputeRoutesAsync(
                    It.IsAny<ComputeRoutesRequest>(),
                    It.IsAny<CallSettings>()))
               .ThrowsAsync(rpcException);

          var service = new GoogleItineraryService(_routesClient.Object, _logger.Object);

          var exception = await Assert.ThrowsAsync<ItineraryServiceException>(() =>
               service.GetItineraryInfoAsync(new GeoPoint(2.35, 48.85), new GeoPoint(2.45, 48.95), CancellationToken.None));

          exception.InnerException.Should().BeSameAs(rpcException);
     }
}