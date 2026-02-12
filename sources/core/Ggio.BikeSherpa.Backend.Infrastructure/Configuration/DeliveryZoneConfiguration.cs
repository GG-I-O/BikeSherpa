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
          builder.Property(z => z.TourPrice).IsRequired();
          builder.Property(z => z.Price).IsRequired();
          builder.OwnsMany(z => z.Cities, cities =>
          {
               cities.ToTable("DeliveryZoneCities");
               cities.WithOwner().HasForeignKey("DeliveryZoneId");
               cities.Property<string>("City").HasColumnName("City");
               cities.HasData(
                    new { DeliveryZoneId = 1, City = "Grenoble" },
                    new { DeliveryZoneId = 2, City = "Échirolles" },
                    new { DeliveryZoneId = 2, City = "Eybens" },
                    new { DeliveryZoneId = 2, City = "Fontaine" },
                    new { DeliveryZoneId = 2, City = "La Tronche" },
                    new { DeliveryZoneId = 2, City = "Poisat" },
                    new { DeliveryZoneId = 2, City = "Saint-Martin-d’Hères" },
                    new { DeliveryZoneId = 2, City = "Saint-Martin-le-Vinoux" },
                    new { DeliveryZoneId = 2, City = "Seyssinet-Pariset" },
                    new { DeliveryZoneId = 2, City = "Seyssins" },
                    new { DeliveryZoneId = 3, City = "" },
                    new { DeliveryZoneId = 4, City = "" }
               );
          });

          builder.HasData(
               new DeliveryZoneEntity
               {
                    Id = 1,
                    Name = "Grenoble",
                    TourPrice = 1,
                    Price = 5
               },
               new DeliveryZoneEntity
               {
                    Id = 2,
                    Name = "Limitrophe",
                    TourPrice = 2.5,
                    Price = 8
               },
               new DeliveryZoneEntity
               {
                    Id = 3,
                    Name = "Périphérie",
                    TourPrice = 5.5,
                    Price = 0
               },
               new DeliveryZoneEntity
               {
                    Id = 4,
                    Name = "Extérieur",
                    TourPrice = 11,
                    Price = 0
               }
          );
     }
}
