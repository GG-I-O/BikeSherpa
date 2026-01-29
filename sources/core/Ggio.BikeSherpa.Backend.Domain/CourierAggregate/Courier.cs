using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate;

public class Courier : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required string FirstName { get; set; }
     public required string LastName { get; set; }
     public required string Code { get; set; }
     public string? Email { get; set; }
     public required string PhoneNumber { get; set; }
     public required Address Address { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }
}