namespace Ggio.BikeSherpa.Backend.Domain;

public record ItineraryResult(double DistanceKm, double TimeMinutes);

public interface IItineraryService
{
     Task<ItineraryResult> GetItineraryInfoAsync(
          string startStepCoordinates,
          string endStepCoordinates,
          CancellationToken cancellationToken = default);
}

