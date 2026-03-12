using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService.Contracts;
using Moq;

namespace Backend.Infrastructure.Tests;

[Trait("Category", "Integration")]
public class ItineraryServiceIntegrationTests
{
     [Fact]
     public async Task GetItineraryInfoAsync_WithValidCoordinates_ReturnsItineraryResult()
     {
          // Arrange
          var mockApi = new Mock<IItineraryApi>();
          const float expectedDistance = 25.5f;
          const float expectedDuration = 30.5f;

          mockApi.Setup(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Itineraire
               {
                    Distance = expectedDistance,
                    Duration = expectedDuration
               });

          var service = new ItineraryService(mockApi.Object);

          // Act
          var result = await service.GetItineraryInfoAsync("2.35,48.85", "2.45,48.95", CancellationToken.None);

          // Assert
          result.DistanceInKm.Should().Be(expectedDistance);
          result.TimeInMinutes.Should().Be(expectedDuration);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_IncludesHighwayBanConstraint()
     {
          // Arrange
          var mockApi = new Mock<IItineraryApi>();

          mockApi.Setup(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Itineraire { Distance = 10f, Duration = 15f });

          var service = new ItineraryService(mockApi.Object);

          // Act
          await service.GetItineraryInfoAsync("2.35,48.85", "2.45,48.95", CancellationToken.None);

          // Assert
          mockApi.Verify(x => x.RouteItinerairePost(
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
          var mockApi = new Mock<IItineraryApi>();
          var cts = new CancellationTokenSource();

          mockApi.Setup(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Itineraire { Distance = 10f, Duration = 15f });

          var service = new ItineraryService(mockApi.Object);

          // Act
          await service.GetItineraryInfoAsync("2.35,48.85", "2.45,48.95", cts.Token);

          // Assert
          mockApi.Verify(x => x.RouteItinerairePost(
                    It.IsAny<RouteBody>(),
                    It.Is<CancellationToken>(c => c == cts.Token)),
               Times.Once);
     }
}