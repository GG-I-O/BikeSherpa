using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public class Customer : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required string Name { get; set; }
     public required string Code { get; set; }
     public string? Siret { get; set; }
     public required string Email { get; set; }
     public required string PhoneNumber { get; set; }
     public required string Address { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }
}
