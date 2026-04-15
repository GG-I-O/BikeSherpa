namespace Ggio.BikeSherpa.Backend.Features.StaticData.PackingSizes.Model;

public record PackingSizeDto
{
     public required string Label { get; init; }
     public required string Value { get; init; }
}
