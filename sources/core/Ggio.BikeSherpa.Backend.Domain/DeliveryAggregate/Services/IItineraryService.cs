namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services;

public record ItineraryResult(double DistanceInKm, double TimeInMinutes);

public interface IItineraryService
{
     Task<ItineraryResult> GetItineraryInfoAsync(
          GeoPoint startStepCoordinates,
          GeoPoint endStepCoordinates,
          CancellationToken cancellationToken = default);
}
