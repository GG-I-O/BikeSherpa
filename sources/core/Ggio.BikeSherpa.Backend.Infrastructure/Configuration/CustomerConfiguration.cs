using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
     public void Configure(EntityTypeBuilder<Customer> builder)
     {
          builder.HasKey(x => x.Id);
          builder.OwnsOne(x => x.Address, address =>
          {
               address.OwnsOne(a => a.Coordinates, coords =>
               {
                    coords.Property(c => c)
                         .HasConversion(
                              v => v.ToString(),
                              v => GeoPoint.FromString(v)
                         )
                         .HasColumnName("Coordinates");
               });
          });
     }
}
