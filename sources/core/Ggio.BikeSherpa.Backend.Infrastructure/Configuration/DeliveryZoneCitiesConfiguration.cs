using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryZoneCitiesConfiguration : IEntityTypeConfiguration<CityEntity>
{
     public void Configure(EntityTypeBuilder<CityEntity> builder)
     {
          builder.ToTable("DeliveryZoneCities");
          builder.HasKey(c => c.Name);
          builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
          builder.HasData(
               // Cities in Grenoble zone
               new { Id = 1, DeliveryZoneId = 1, Name = "Grenoble" },
               // Cities in Border zone
               new { Id = 2, DeliveryZoneId = 2, Name = "Échirolles" },
               new { Id = 3, DeliveryZoneId = 2, Name = "Eybens" },
               new { Id = 4, DeliveryZoneId = 2, Name = "Fontaine" },
               new { Id = 5, DeliveryZoneId = 2, Name = "La Tronche" },
               new { Id = 6, DeliveryZoneId = 2, Name = "Poisat" },
               new { Id = 7, DeliveryZoneId = 2, Name = "Saint-Martin-d’Hères" },
               new { Id = 8, DeliveryZoneId = 2, Name = "Saint-Martin-le-Vinoux" },
               new { Id = 9, DeliveryZoneId = 2, Name = "Seyssinet-Pariset" },
               new { Id = 10, DeliveryZoneId = 2, Name = "Seyssins" },
               // Cities in Periphery zone
               new { Id = 11, DeliveryZoneId = 3, Name = "Sassenage" },
               new { Id = 12, DeliveryZoneId = 3, Name = "Saint-Égrève" },
               new { Id = 13, DeliveryZoneId = 3, Name = "Meylan" },
               new { Id = 14, DeliveryZoneId = 3, Name = "Gières" },
               new { Id = 15, DeliveryZoneId = 3, Name = "Bresson" },
               new { Id = 16, DeliveryZoneId = 3, Name = "Le Pont-de-Claix" }
          );
     }
}
