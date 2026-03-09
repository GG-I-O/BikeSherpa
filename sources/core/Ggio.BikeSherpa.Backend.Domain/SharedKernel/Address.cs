using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Domain.SharedKernel;

public class Address
{
     [UsedImplicitly]
     public required string Name { get; set; }

     [UsedImplicitly]
     public required string StreetInfo { get; set; }

     public string? Complement { get; init; }

     [UsedImplicitly]
     public required string Postcode { get; set; }

     [UsedImplicitly]
     public required string City { get; set; }
     [UsedImplicitly]
     public required string Coordinates { get; set; }
}
