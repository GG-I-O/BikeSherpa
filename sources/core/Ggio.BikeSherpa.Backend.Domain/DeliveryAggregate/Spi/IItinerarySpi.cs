namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

public record ItineraryResult(double DistanceInKm, double TimeInMinutes);

public interface IItinerarySpi
{
     Task<ItineraryResult> GetItineraryInfoAsync(
          GeoPoint startStepCoordinates,
          GeoPoint endStepCoordinates,
          CancellationToken cancellationToken = default);
}
