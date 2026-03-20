namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;

public record ItineraryResult(double DistanceInKm, double TimeInMinutes);

public interface IItinerarySpi
{
     Task<ItineraryResult> GetItineraryInfoAsync(
          GeoPoint startStepCoordinates,
          GeoPoint endStepCoordinates,
          CancellationToken cancellationToken = default);
}
