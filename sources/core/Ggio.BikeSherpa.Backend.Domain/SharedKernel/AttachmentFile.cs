namespace Ggio.BikeSherpa.Backend.Domain.SharedKernel;

public record AttachmentFile
{
     public required string Path { get; init; }
     public required string DomainType { get; init; }
}
