using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryZoneConfiguration : IEntityTypeConfiguration<DeliveryZoneEntity>
{
     public void Configure(EntityTypeBuilder<DeliveryZoneEntity> builder)
     {
          builder.ToTable("DeliveryZones");
          builder.HasKey(z => z.Id);
          builder.Property(z => z.Name).HasMaxLength(100).IsRequired();
          builder.HasMany(z => z.Cities)
               .WithOne()
               .HasForeignKey("DeliveryZoneId")
               .OnDelete(DeleteBehavior.Cascade);

          builder.Navigation(z => z.Cities).AutoInclude();

          builder.HasData(
               new DeliveryZoneEntity { Id = 1, Name = "Grenoble" },
               new DeliveryZoneEntity { Id = 2, Name = "Limitrophe" },
               new DeliveryZoneEntity { Id = 3, Name = "Périphérie" },
               new DeliveryZoneEntity { Id = 4, Name = "Extérieur" }
          );
     }
}
