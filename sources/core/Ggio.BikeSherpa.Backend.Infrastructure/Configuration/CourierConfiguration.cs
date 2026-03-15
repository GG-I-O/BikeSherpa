using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
     public void Configure(EntityTypeBuilder<Courier> builder)
     {
          builder.HasKey(x => x.Id);
          builder.OwnsOne(x => x.Address, address =>
          {
               address.Property(a => a.Name).HasMaxLength(200).IsRequired();
               address.Property(a => a.StreetInfo).HasMaxLength(200).IsRequired();
               address.Property(a => a.Complement).HasMaxLength(200);
               address.Property(a => a.Postcode).HasMaxLength(5).IsRequired();
               address.Property(a => a.City).HasMaxLength(100).IsRequired();
               address.Property(a => a.Coordinates).HasConversion(
                    c => c.ToString(),
                    s => GeoPoint.TryParse(s)).IsRequired();
          });
     }
}
