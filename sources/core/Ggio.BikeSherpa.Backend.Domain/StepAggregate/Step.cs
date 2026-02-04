using Ggio.DddCore;

namespace Ggio.BikeSherpa.Backend.Domain.StepAggregate;

public class Step : EntityBase, IAggregateRoot
{
     public required string Type { get; set; }
     
     public required Address Address { get; set; }
     
     public required double Distance { get; set; }
     
     public required double Price { get; set; }
     
     public string? CourierId { get; set; }
     
     public string? Comment { get; set; }
     
     public string[]? filePaths { get; set; }
     
     public required DateTime ContractDate { get; set; }
     
     public DateTime EstimatedDeliveryDate { get; set; }
     
     public DateTime RealDeliveryDate { get; set; }
     
     public DateTimeOffset CreatedAt { get; set; }
     
     public DateTimeOffset UpdatedAt { get; set; }
}
