using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required DeliveryStatus Status { get; set; }
     public required string Code { get; set; }
     public required Guid CustomerId { get; set; }
     public required double TotalPrice { get; set; }
     public required string ReportId { get; set; }
     public List<DeliveryStep> Steps { get; set; } = [];
     public required string[] Details { get; set; }
     public required string Packing { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }
}
