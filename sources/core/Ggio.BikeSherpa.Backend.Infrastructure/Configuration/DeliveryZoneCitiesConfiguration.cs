using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryZoneCitiesConfiguration : IEntityTypeConfiguration<City>
{
     public void Configure(EntityTypeBuilder<City> builder)
     {
          builder.ToTable("DeliveryZoneCities");
          builder.HasKey(c => c.Id);
          builder.Property(c => c.Id).ValueGeneratedOnAdd();
          builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
          builder.HasData(
               new { DeliveryZoneName = "Grenoble", Name = "Grenoble" },
               new { DeliveryZoneName = "Limitrophe", Name = "Échirolles" },
               new { DeliveryZoneName = "Limitrophe", Name = "Eybens" },
               new { DeliveryZoneName = "Limitrophe", Name = "Fontaine" },
               new { DeliveryZoneName = "Limitrophe", Name = "La Tronche" },
               new { DeliveryZoneName = "Limitrophe", Name = "Poisat" },
               new { DeliveryZoneName = "Limitrophe", Name = "Saint-Martin-d’Hères" },
               new { DeliveryZoneName = "Limitrophe", Name = "Saint-Martin-le-Vinoux" },
               new { DeliveryZoneName = "Limitrophe", Name = "Seyssinet-Pariset" },
               new { DeliveryZoneName = "Limitrophe", Name = "Seyssins" },
               new { DeliveryZoneName = "Périphérie", Name = "Sassenage" },
               new { DeliveryZoneName = "Périphérie", Name = "Saint-Égrève" },
               new { DeliveryZoneName = "Périphérie", Name = "Meylan" },
               new { DeliveryZoneName = "Périphérie", Name = "Gières" },
               new { DeliveryZoneName = "Périphérie", Name = "Bresson" },
               new { DeliveryZoneName = "Périphérie", Name = "Le Pont-de-Claix" }
          );
     }
}
