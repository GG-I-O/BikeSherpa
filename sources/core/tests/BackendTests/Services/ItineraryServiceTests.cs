using System.Text.Json;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Infrastructure.GeoService;
using Ggio.BikeSherpa.Backend.Services;
using Refit;

namespace BackendTests.Services;

[Trait("Category", "Integration")]
public class ItineraryServiceTests
{
     [Fact]
     public async Task GetItineraryInfoAsync_WithRealApi_ReturnsValidData()
     {
          // Arrange
          const string geoServiceBaseUrl = "https://data.geopf.fr/navigation";
          var httpClient = new HttpClient(new LoggingHandler())
          {
               BaseAddress = new Uri(geoServiceBaseUrl)
          };

          var itineraryApi = RestService.For<IItineraryApi>(httpClient);

          var service = new ItineraryService(itineraryApi);

          // Act
          try
          {
               var result = await service.GetItineraryInfoAsync("2.35,48.85", "2.45,48.95", CancellationToken.None);

               // Assert
               result.DistanceInKm.Should().Be(24.4369);
               result.TimeInMinutes.Should().Be(37.669999999999995);

               Console.WriteLine($"Distance: {result.DistanceInKm} km");
               Console.WriteLine($"Time: {result.TimeInMinutes} minutes");
          }
          catch (ApiException ex)
          {
               Console.WriteLine($"API Error: {ex.StatusCode}");
               var content = await ex.GetContentAsAsync<JsonElement>();
               Console.WriteLine($"Content: {content}");
          }
     }
}
