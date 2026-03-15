using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.Tests;

[Trait("Category", "E2E")]
public class ItineraryServiceE2ETest
{
     private static IItineraryService MakeSut()
     {
          var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
          var services = new ServiceCollection();
          services.AddLogging();
          services.AddBackendInfrastructure(configuration);
          var serviceProvider = services.BuildServiceProvider();
          var service = serviceProvider.GetRequiredService<IItineraryService>();

          return service;
     }

     [Fact]
     public async Task GetItineraryInfoAsync_WithRealApi_ReturnsValidData()
     {
          // Arrange
          var sut = MakeSut();

          // Act
          var result = await sut.GetItineraryInfoAsync(new GeoPoint(2.35, 48.85), new GeoPoint(2.45, 48.95), CancellationToken.None);

          // Assert
          result.DistanceInKm.Should().BeGreaterThan(0);
          result.TimeInMinutes.Should().BeGreaterThan(0);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_WithRealApi_ThrowsItineraryServiceException_WithIdenticalCoordinates()
     {
          // Arrange
          var sut = MakeSut();

          // Act
          var ex = await Assert.ThrowsAsync<ItineraryServiceException>(() =>
               sut.GetItineraryInfoAsync(new GeoPoint(2.35, 48.85), new GeoPoint(2.35, 48.85), CancellationToken.None));

          // Assert
          ex.Message.Should().Be("Coordonnées de départ et d’arrivée identiques");
     }
}
