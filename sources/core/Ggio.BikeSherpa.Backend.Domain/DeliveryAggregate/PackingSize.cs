namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record PackingSize(
     string Name,
     int MaxWeight,
     int MaxPackageLength,
     double TourPrice,
     double Price
     );