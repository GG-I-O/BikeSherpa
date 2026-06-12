using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryZoneConfiguration : IEntityTypeConfiguration<DeliveryZone>
{
     public void Configure(EntityTypeBuilder<DeliveryZone> builder)
     {
          builder.ToTable("DeliveryZones");
          builder.HasKey(z => z.Name);
          builder.Property(z => z.Name).HasMaxLength(100).IsRequired();
          builder.HasMany(z => z.Cities)
               .WithOne()
               .HasForeignKey("DeliveryZoneName")
               .OnDelete(DeleteBehavior.Cascade);

          builder.Navigation(z => z.Cities).AutoInclude();

          builder.HasData(
               new { Name = "Centre", TourPrice = 5.0, SimplePrice = 1.0 },
               new { Name = "Limitrophe", TourPrice = 8.0, SimplePrice = 2.5 },
               new { Name = "Périphérie", TourPrice = 0.0, SimplePrice = 5.5 },
               new { Name = DeliveryZoneRepository.FallbackZoneForUnknownCity, TourPrice = 0.0, SimplePrice = 11.0 }
          );
     }
}
