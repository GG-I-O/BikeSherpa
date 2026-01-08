namespace Ggio.BikeSherpa.Backend.Model;

public record ThrownValidationError
{
     public required string Origin { get; init; }
     public required string Message { get; init; }
}
