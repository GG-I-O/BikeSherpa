using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class PackingSizeConfiguration : IEntityTypeConfiguration<PackingSize>
{
     public void Configure(EntityTypeBuilder<PackingSize> builder)
     {
          builder.ToTable("PackingSizes");
          builder.HasKey(s => s.Name);
          builder.Property(u => u.Order).IsRequired();
          builder.Property(u => u.Label).IsRequired();
          builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
          builder.Property(s => s.TourPrice).IsRequired();
          builder.Property(s => s.SimplePrice).IsRequired();
          builder.HasData(
               new
               {
                    Name = "S",
                    Order = 1,
                    Label = "S = 0kg à 2.9kg & L max 45cm",
                    TourPrice = 0.0,
                    SimplePrice = 3.0
               },
               new
               {
                    Name = "M",
                    Order = 2,
                    Label = "M = 3kg à 9.9kg & L max 100cm",
                    TourPrice = 2.0,
                    SimplePrice = 5.0
               },
               new
               {
                    Name = "L",
                    Order = 3,
                    Label = "L = 10kg à 19.9kg & L max 120cm",
                    TourPrice = 4.0,
                    SimplePrice = 7.0
               },
               new
               {
                    Name = "XL",
                    Order = 4,
                    Label = "XL = 20kg à 29.9kg & L max 120cm",
                    TourPrice = 6.0,
                    SimplePrice = 9.0
               },
               new
               {
                    Name = "XXL",
                    Order = 5,
                    Label = "XXL = 30kg à 39.9kg & L max 120cm",
                    TourPrice = 8.0,
                    SimplePrice = 11.0
               },
               new
               {
                    Name = "XXXL",
                    Order = 6,
                    Label = "XXXL = supérieur à 40kg (sur devis)",
                    TourPrice = 10.0,
                    SimplePrice = 13.0
               }
          );
     }
}
