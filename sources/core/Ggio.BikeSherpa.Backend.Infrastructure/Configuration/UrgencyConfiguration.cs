using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class UrgencyConfiguration : IEntityTypeConfiguration<Urgency>
{
     public void Configure(EntityTypeBuilder<Urgency> builder)
     {
          builder.ToTable("Urgencies");
          builder.HasKey(u => u.Id);
          builder.Property(u => u.Id).ValueGeneratedOnAdd();
          builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
          builder.Property(u => u.PriceCoefficient).IsRequired();
          builder.HasData(
               new { Name = "Eco", PriceCoefficient = 0.75 },
               new { Name = "Standard", PriceCoefficient = 1.25 },
               new { Name = "Urgent", PriceCoefficient = 2 }
          );
     }
}
