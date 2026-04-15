using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.Model;

public record PricingStrategyDto
{
     [UsedImplicitly]
     public required string Label { get; set; }
     public required PricingStrategy Value { get; init; }
};
