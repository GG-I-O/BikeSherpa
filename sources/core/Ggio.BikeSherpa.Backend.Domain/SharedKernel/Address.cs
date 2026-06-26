using System.Text;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
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
     public required GeoPoint Coordinates { get; set; }

     public string? Phone { get; init; }

     public string GetFullAddress()
     {
          var builder = new StringBuilder();
          builder.Append(StreetInfo);
          if (!string.IsNullOrEmpty(Complement))
          {
               builder.AppendLine(Complement);
          }
          else
          {
               builder.AppendLine();
          }

          builder.AppendLine(Postcode);
          builder.AppendLine(City);
          return builder.ToString();
     }
}
