using Microsoft.EntityFrameworkCore;

namespace Ggio.BikeSherpa.Backend.Domain;

[Owned]
public class Address
{
     public required string name { get; set; }
     public required string streetInfo { get; set; }
     public string? complement { get; set; }
     public required string postcode { get; set; }
     public required string city { get; set; }
}
