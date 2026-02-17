using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryZoneConfiguration : IEntityTypeConfiguration<DeliveryZone>
{
     public void Configure(EntityTypeBuilder<DeliveryZone> builder)
     {
          builder.ToTable("DeliveryZones");
          builder.HasKey(z => z.Name);
          builder.Property(z => z.Id).ValueGeneratedOnAdd();
          builder.Property(z => z.Name).HasMaxLength(100).IsRequired();
          builder.HasMany(z => z.Cities)
               .WithOne()
               .HasForeignKey("DeliveryZoneName")
               .OnDelete(DeleteBehavior.Cascade);

          builder.Navigation(z => z.Cities).AutoInclude();

          builder.HasData(
               new { Name = "Grenoble" },
               new { Name = "Limitrophe" },
               new { Name = "Périphérie" },
               new { Name = "Extérieur" }
          );
     }
}
