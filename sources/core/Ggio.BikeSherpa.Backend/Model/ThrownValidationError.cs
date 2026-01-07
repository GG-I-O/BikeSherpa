namespace Ggio.BikeSherpa.Backend.Model;

public record ThrownValidationError
{
     public required string PropertyName { get; init; }
     public required string ErrorMessage { get; init; }
}
