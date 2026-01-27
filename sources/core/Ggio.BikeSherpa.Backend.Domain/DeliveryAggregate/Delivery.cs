using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot
{
     public required string Code { get; set; }
     
     public required Customer Customer { get; set; }
     
     public required double TotalPrice { get; set; }
     
     public required string ReportId { get; set; }
     
     public required string[] Steps { get; set; }
     
     public required string[] Details { get; set; }
     
     public required string Packing { get; set; }
     
     public DateTimeOffset ContractDate { get; set; }
     
     public DateTimeOffset CreatedAt { get; set; }
     
     public DateTimeOffset UpdatedAt { get; set; }
}
