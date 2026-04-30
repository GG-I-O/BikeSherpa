namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record PackingSize(
     string Name,
     int Order,
     string Label,
     int MaxWeight,
     int MaxPackageLength,
     double TourPrice,
     double Price
);
