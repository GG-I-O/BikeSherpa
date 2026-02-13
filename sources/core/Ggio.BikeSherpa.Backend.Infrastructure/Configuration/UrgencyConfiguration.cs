using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class UrgencyConfiguration : IEntityTypeConfiguration<UrgencyEntity>
{
     public void Configure(EntityTypeBuilder<UrgencyEntity> builder)
     {
          builder.ToTable("Urgencies");
          builder.HasKey(p => p.Id);
          builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
          builder.Property(p => p.PriceCoefficient).IsRequired();
          builder.HasData(
               new UrgencyEntity
               {
                    Id = 1,
                    Name = "Eco",
                    PriceCoefficient = 2.75
               },
               new UrgencyEntity
               {
                    Id = 2,
                    Name = "Standard",
                    PriceCoefficient = 1.25
               },
               new UrgencyEntity
               {
                    Id = 2,
                    Name = "Urgent",
                    PriceCoefficient = 2
               }
          );
     }
}
