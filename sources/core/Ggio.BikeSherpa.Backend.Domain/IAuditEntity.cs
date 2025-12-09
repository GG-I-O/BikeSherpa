namespace Ggio.BikeSherpa.Backend.Domain;

public interface IAuditEntity
{
     DateTimeOffset CreatedAt { get; set; }
     DateTimeOffset UpdatedAt { get; set; }
}
