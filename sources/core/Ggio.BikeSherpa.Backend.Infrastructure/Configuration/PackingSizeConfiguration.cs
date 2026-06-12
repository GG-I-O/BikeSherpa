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
                    Label = "S = jusqu'à 3kg / 45cm",
                    TourPrice = 0.0,
                    Price = 3.0
               },
               new
               {
                    Name = "M",
                    Order = 2,
                    Label = "M = jusqu'à 10kg / 100cm",
                    TourPrice = 2.0,
                    Price = 5.0
               },
               new
               {
                    Name = "L",
                    Order = 3,
                    Label = "L = jusqu'à 20kg / 120cm",
                    TourPrice = 4.0,
                    Price = 7.0
               },
               new
               {
                    Name = "XL",
                    Order = 4,
                    Label = "XL = jusqu'à 30kg / 120cm",
                    TourPrice = 6.0,
                    Price = 9.0
               },
               new
               {
                    Name = "XXL",
                    Order = 5,
                    Label = "XXL = jusqu'à 40kg (sur devis)",
                    TourPrice = 8.0,
                    Price = 11.0
               },
               new
               {
                    Name = "XXXL",
                    Order = 6,
                    Label = "XXXL = jusqu'à 50kg (sur devis)",
                    TourPrice = 10.0,
                    Price = 13.0
               },
               new
               {
                    Name = "XXXXL",
                    Order = 7,
                    Label = "XXXXL = jusqu'à 60kg (sur devis)",
                    TourPrice = 12.0,
                    Price = 15.0
               }
          );
     }
}
