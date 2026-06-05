namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record Urgency(
     string Name,
     int Order,
     string Label,
     double PriceCoefficient,
     TimeSpan? AddTimeLimit,
     TimeSpan? FixedTimeLimit,
     int LastHourToOrder
);
