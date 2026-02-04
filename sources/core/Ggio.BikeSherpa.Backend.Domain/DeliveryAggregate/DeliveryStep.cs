using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public class DeliveryStep : EntityBase<Guid>, IAuditEntity
{
     public required StepType StepType { get; set; }
     public required int Order { get; set; }
     public required Address Address { get; set; }
     public required double Distance { get; set; }
     public required double Price { get; set; }
     public Guid? CourierId { get; set; }
     public string? Comment { get; set; }
     public string[]? FilePaths { get; set; }
     public required DateTimeOffset ContractDate { get; set; }
     public required DateTimeOffset EstimatedDeliveryDate { get; set; }
     public DateTimeOffset? RealDeliveryDate { get; set; }
     public DateTimeOffset CreatedAt { get; set; }
     public DateTimeOffset UpdatedAt { get; set; }
}
