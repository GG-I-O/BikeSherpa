namespace Ggio.BikeSherpa.Backend.Domain;

public record ItineraryResult(double DistanceInKm, double TimeInMinutes);

public interface IItineraryService
{
     Task<ItineraryResult> GetItineraryInfoAsync(
          string startStepCoordinates,
          string endStepCoordinates,
          CancellationToken cancellationToken = default);
}

