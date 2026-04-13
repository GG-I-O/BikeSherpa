namespace Ggio.BikeSherpa.Backend.Features.Public.PackingSizes.Model;

public record PackingSizeDto
{
     public required string Label { get; set; }
     public required string Value { get; set; }
}
