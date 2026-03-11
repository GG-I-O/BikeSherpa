using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService;
using Microsoft.Extensions.Configuration;
using Refit;

namespace BackendTests.Services;

[Trait("Category", "Integration")]
public class ItineraryServiceTests
{
     [Fact]
     public async Task GetItineraryInfoAsync_WithRealApi_ReturnsValidData()
     {
          // Arrange
          var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

          var itineraryApi = RestService.For<IItineraryApi>(configuration["ItineraryService:BaseUrl"]!);
          var service = new ItineraryService(itineraryApi);

          // Act
          var result = await service.GetItineraryInfoAsync("2.35,48.85", "2.45,48.95", CancellationToken.None);

          // Assert
          result.DistanceInKm.Should().BeGreaterThan(0);
          result.TimeInMinutes.Should().BeGreaterThan(0);

          Console.WriteLine($"Distance : {result.DistanceInKm} km");
          Console.WriteLine($"Temps de trajet : {result.TimeInMinutes} minutes");
     }
}
