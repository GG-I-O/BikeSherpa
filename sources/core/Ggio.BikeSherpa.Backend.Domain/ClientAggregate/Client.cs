using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.ClientAggregate;

public class Client: EntityBase<Guid>, IAggregateRoot
{
     public string Name { get; set; }
     public string Code { get; set; }
     public string? Siret { get; set; }
     public string Email { get; set; }
     public string PhoneNumber { get; set; }
     public string Address { get; set; }
}
