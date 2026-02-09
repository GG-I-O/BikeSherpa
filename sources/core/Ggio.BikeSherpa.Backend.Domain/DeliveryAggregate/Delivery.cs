using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class Delivery : EntityBase<Guid>, IAggregateRoot, IAuditEntity
{
     public required PricingStrategy PricingStrategy { get; set; }
     public required DeliveryStatus Status { get; set; }
     public Address? PickupAddress { get; set; }
     public DeliveryZone? PickupZone { get; set; }
     public required string Code { get; set; }
     public required Guid CustomerId { get; set; }
     public required Urgency Urgency { get; set; }
     public required double TotalPrice { get; set; }
     public required Guid ReportId { get; set; }
     public List<DeliveryStep> Steps { get; set; } = [];
     public required string[] Details { get; set; }
     public required double Weight { get; set; }
     public required int Length { get; set; }
     public required Packing Packing { get; set; }
     public required DateTimeOffset ContractDate { get; set; }
     public required DateTimeOffset StartDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }
}
