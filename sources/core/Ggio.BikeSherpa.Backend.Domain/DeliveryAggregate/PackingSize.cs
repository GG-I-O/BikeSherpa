namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record PackingSize(
     string Name,
     int MaxWeight,
     int TourMaxLength,
     int MaxLength,
     int TourPrice,
     int Price
     );