using Ggio.BikeSherpa.Backend.Domain.SharedKernel;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public record ProofOfDelivery
{
     public required string DeliveryLabel { get; init; }
     public required List<ProofOfDeliveryStep> Steps { get; init; }
}

public record ProofOfDeliveryStep
{
     public Address? Address { get; init; }
     public DateTimeOffset? DeliveryDate { get; init; }
     public string? Receiver { get; init; }
     public required List<AttachmentFile> AttachmentFiles { get; init; }
}