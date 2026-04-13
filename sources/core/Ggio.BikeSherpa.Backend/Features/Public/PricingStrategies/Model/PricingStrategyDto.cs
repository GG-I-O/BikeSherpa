using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Features.Public.PricingStrategies.Model;

public record PricingStrategyDto
{
     public required string Label { get; set; }
     public required PricingStrategy Value { get; set; }
};
