namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record PackingSize(
     string Name,
     int Order,
     string Label,
     double TourPrice,
     double SimplePrice
);
