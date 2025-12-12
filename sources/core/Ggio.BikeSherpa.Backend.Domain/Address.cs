using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Domain;

public class Address
{
     [UsedImplicitly]
     public required string name { get; set; }
     
     [UsedImplicitly]
     public required string streetInfo { get; set; }
     
     public string? complement { get; set; }
     
     [UsedImplicitly]
     public required string postcode { get; set; }
     
     [UsedImplicitly]
     public required string city { get; set; }
}
