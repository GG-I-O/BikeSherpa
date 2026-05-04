using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class UrgencyConfiguration : IEntityTypeConfiguration<Urgency>
{
     public void Configure(EntityTypeBuilder<Urgency> builder)
     {
          builder.ToTable("Urgencies");
          builder.HasKey(u => u.Name);
          builder.Property(u => u.Order).IsRequired();
          builder.Property(u => u.Label).IsRequired();
          builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
          builder.Property(u => u.PriceCoefficient).IsRequired();
          builder.HasData(
               new { Name = "Eco", Order = 1, Label = "Avant 17h le jour-même (Eco)", PriceCoefficient = 0.75 },
               new { Name = "Standard", Order = 2, Label = "2h30 (Standard)", PriceCoefficient = 1.25 },
               new { Name = "Urgent", Order = 3, Label = "1h (Urgent)", PriceCoefficient = 2.0 }
          );
     }
}
