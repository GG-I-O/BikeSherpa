using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.Tests;

[Trait("Category", "E2E")]
public class ItineraryServiceE2ETest
{
     [Fact]
     public async Task GetItineraryInfoAsync_WithRealApi_ReturnsValidData()
     {
          // Arrange
          var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
          var services = new ServiceCollection();
          services.AddBackendInfrastructure(configuration);
          var serviceProvider = services.BuildServiceProvider();
          var service = serviceProvider.GetRequiredService<IItineraryService>();

          // Act
          var result = await service.GetItineraryInfoAsync("2.35,48.85", "2.45,48.95", CancellationToken.None);

          // Assert
          result.DistanceInKm.Should().BeGreaterThan(0);
          result.TimeInMinutes.Should().BeGreaterThan(0);

          // Debug
          Console.WriteLine($"Distance : {result.DistanceInKm} km");
          Console.WriteLine($"Temps de trajet : {result.TimeInMinutes} minutes");
     }
}
