using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Events;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;

public enum DeliveryType
{
     Simple,
     Tour
}

public class Customer : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required string Name { get; set; }
     public required string Code
     {
          get;
          [Obsolete("don't use this property, use SetCode instead")]
          set;
     }
     public string? Siret { get; set; }
     public string? VatNumber { get; set; }
     public required string Email { get; set; }
     public required string PhoneNumber { get; set; }
     public required Address Address { get; set; }

     public DeliveryType? DefaultDeliveryType { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }

     public void SetCode(string code)
     {
          if (code != Code)
          {
               Code = code;
               RegisterDomainEvent(new CustomerCodeChanged(Id, code));
          }
     }
}
