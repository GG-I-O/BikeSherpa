namespace Ggio.BikeSherpa.Backend.Domain;

public record RouteInformation(double Distance, double Duration);

public interface IGeoService
{
     Task<RouteInformation> GetDistanceAndTimeBetweenSteps(string startLocation, string endLocation);
}
