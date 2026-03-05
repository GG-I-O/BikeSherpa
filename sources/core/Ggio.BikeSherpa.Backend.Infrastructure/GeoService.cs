using System.Text.Json;
using System.Text.Json.Serialization;
using Ggio.BikeSherpa.Backend.Domain;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class GeoServiceResponse
{
     [JsonPropertyName("distance")]
     public double Distance { get; init; }
     [JsonPropertyName("duration")]
     public double Duration { get; init; }
}

public class GeoService(HttpClient client) : IGeoService
{
     public async Task<RouteInformation> GetDistanceAndTimeBetweenSteps(string startLocation, string endLocation)
     {
          var url = $"https://data.geopf.fr/navigation/itineraire?resource=bdtopo-osrm" +
          $"&start={startLocation}" +
          $"&end={endLocation}" +
          "&profile=car" +
          "&optimization=fastest" +
          "&constraints=%7B%22constraintType%22:%22banned%22,%22key%22:%22wayType%22,%22operator%22:%22=%22,%22value%22:%22autoroute%22%7D" +
          "&getSteps=false" +
          "&distanceUnit=kilometer" +
          "&timeUnit=minute";

          var response = await client.GetAsync(url);

          if (!response.IsSuccessStatusCode)
          {
               return new RouteInformation(0, 0);
          }

          var jsonResponse = await response.Content.ReadAsStringAsync();

          var data = JsonSerializer.Deserialize<GeoServiceResponse>(jsonResponse);

          return new RouteInformation(data?.Distance ?? 0, data?.Duration ?? 0);
     }
}
