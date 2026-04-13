namespace Ggio.BikeSherpa.Backend.Features.StaticData.Urgencies.Model;

public record UrgencyDto
{
     public required string Label { get; init; }
     public required string Value { get; init; }
}
