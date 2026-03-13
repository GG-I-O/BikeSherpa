using System.Net;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

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
          var result = await sut.GetItineraryInfoAsync("2.35,48.85", "2.45,48.95", CancellationToken.None);

          // Assert
          result.DistanceInKm.Should().BeGreaterThan(0);
          result.TimeInMinutes.Should().BeGreaterThan(0);
     }

     [Fact]
     public async Task GetItineraryInfoAsync_WithRealApi_ThrowsApiException_WithInvalidRequest()
     {
          // Arrange
          var sut = MakeSut();

          // Act
          var ex = await Assert.ThrowsAsync<ApiException>(() =>
               sut.GetItineraryInfoAsync("a", "b", CancellationToken.None));

          // Assert
          ex.StatusCode.Should().Be(HttpStatusCode.BadRequest);
          ex.Content.Should().NotBeNullOrEmpty();
     }

     [Fact]
     public async Task GetItineraryInfoAsync_WithRealApi_ThrowsItineraryServiceException_WithIdenticalCoordinates()
     {
          // Arrange
          var sut = MakeSut();

          // Act
          var ex = await Assert.ThrowsAsync<ItineraryServiceException>(() =>
               sut.GetItineraryInfoAsync("2.35,48.85", "2.35,48.85", CancellationToken.None));

          // Assert
          ex.Message.Should().Be("Coordonnées de départ et d’arrivée identiques");
     }
}
