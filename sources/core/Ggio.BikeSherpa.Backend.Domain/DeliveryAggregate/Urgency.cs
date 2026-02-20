namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record Urgency(
     int Id,
     string Name,
     double PriceCoefficient
);
