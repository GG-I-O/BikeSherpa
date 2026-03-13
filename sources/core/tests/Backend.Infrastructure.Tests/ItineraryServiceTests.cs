using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService.Contracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Infrastructure.Tests;

public class ItineraryServiceTests
{
     private readonly Mock<IItineraryApi> _sut = new();
     private readonly Mock<ILogger<ItineraryService>> _logger = new();
     private readonly CancellationTokenSource _cts = new();

     [Fact]
     public async Task GetItineraryInfoAsync_WithValidCoordinates_ReturnsItineraryResult()
     {
          // Arrange
          const float expectedDistance = 25.5f;
          const float expectedDuration = 30.5f;

          _sut.Setup(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Itineraire
               {
                    Distance = expectedDistance,
                    Duration = expectedDuration
               });

          var service = new ItineraryService(_sut.Object, _logger.Object);

          // Act
          var result = await service.GetItineraryInfoAsync(new GeoPoint(2.35,48.85), new GeoPoint(2.45,48.95), CancellationToken.None);

          // Assert
          result.DistanceInKm.Should().Be(expectedDistance);
          result.TimeInMinutes.Should().Be(expectedDuration);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_IncludesHighwayBanConstraint()
     {
          // Arrange
          _sut.Setup(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Itineraire { Distance = 10f, Duration = 15f });

          var service = new ItineraryService(_sut.Object, _logger.Object);

          // Act
          await service.GetItineraryInfoAsync(new GeoPoint(2.35,48.85), new GeoPoint(2.45,48.95), CancellationToken.None);

          // Assert
          _sut.Verify(x => x.RouteItinerairePost(
                    It.Is<RouteBody>(body =>
                         body.Constraints != null &&
                         body.Constraints.Count == 1 &&
                         body.Constraints.First().ConstraintType == ConstraintType.Banned &&
                         body.Constraints.First().Key == "waytype" &&
                         body.Constraints.First().Operator == "=" &&
                         body.Constraints.First().Value == "autoroute"
                    ),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_WithCancellationToken_PassesTokenToApi()
     {
          // Arrange
          _sut.Setup(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Itineraire { Distance = 10f, Duration = 15f });

          var service = new ItineraryService(_sut.Object, _logger.Object);

          // Act
          await service.GetItineraryInfoAsync(new GeoPoint(2.35,48.85), new GeoPoint(2.45,48.95), _cts.Token);

          // Assert
          _sut.Verify(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.Is<CancellationToken>(c => c == _cts.Token)),
               Times.Once);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_ThrowsItineraryServiceException_WhenCoordinatesAreIdentical()
     {
          // Arrange
          var service = new ItineraryService(_sut.Object, _logger.Object);

          // Act & Assert
          var ex = await Assert.ThrowsAsync<ItineraryServiceException>(() =>
               service.GetItineraryInfoAsync(new GeoPoint(2.35,48.85), new GeoPoint(2.35,48.85), CancellationToken.None));

          ex.Message.Should().Be("Coordonnées de départ et d’arrivée identiques");

          _sut.Verify(x => x.RouteItinerairePost(It.IsAny<RouteBody>(), It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_ThrowsItineraryServiceException_WhenApiReturnsNull()
     {
          // Arrange
          _sut.Setup(x => x.RouteItinerairePost(It.IsAny<RouteBody>(), It.IsAny<CancellationToken>()))!
               .ReturnsAsync(null as Itineraire);

          var service = new ItineraryService(_sut.Object, _logger.Object);

          // Act & Assert
          var ex = await Assert.ThrowsAsync<ItineraryServiceException>(() =>
               service.GetItineraryInfoAsync(new GeoPoint(2.35,48.85), new GeoPoint(2.45,48.95), CancellationToken.None));

          ex.Message.Should().Be("Aucune réponse de l'API");
     }
}
