namespace Ggio.BikeSherpa.Backend.Features.Public.Urgencies.Model;

public record UrgencyDto
{
     public required string Label { get; set; }
     public required string Value { get; set; }
}
