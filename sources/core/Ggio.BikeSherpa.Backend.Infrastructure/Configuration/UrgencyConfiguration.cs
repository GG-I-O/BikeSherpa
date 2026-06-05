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
          builder.Property(u => u.AddTimeLimit).IsRequired(false);
          builder.Property(u => u.FixedTimeLimit).IsRequired(false);
          builder.Property(u => u.LastHourToOrder).IsRequired();
          builder.HasData(
               new
               {
                    Name = "Eco",
                    Order = 1,
                    Label = "Avant 17h le jour-même (Eco)",
                    PriceCoefficient = 0.75,
                    FixedTimeLimit = new TimeSpan(17, 0, 0),
                    LastHourToOrder = 12
               },
               new
               {
                    Name = "Standard",
                    Order = 2,
                    Label = "2h30 (Standard)",
                    PriceCoefficient = 1.25,
                    AddTimeLimit = new TimeSpan(2, 30, 0),
                    LastHourToOrder = 15
               },
               new
               {
                    Name = "Urgent",
                    Order = 3,
                    Label = "1h (Urgent)",
                    PriceCoefficient = 2.0,
                    AddTimeLimit = new TimeSpan(1, 0, 0),
                    LastHourToOrder = 20
               }
          );
     }
}
