namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record Delay
{
     public required double Price { get; set; }
     public required string Id { get; set; }
     public required string Label { get; set; }
}
