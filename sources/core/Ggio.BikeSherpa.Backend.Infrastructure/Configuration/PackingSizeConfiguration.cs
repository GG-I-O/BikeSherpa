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
          builder.Property(s => s.MaxWeight).IsRequired();
          builder.Property(s => s.MaxPackageLength).IsRequired();
          builder.Property(s => s.TourPrice).IsRequired();
          builder.Property(s => s.Price).IsRequired();
          builder.HasData(
               new
               {
                    Name = "S",
                    Order = 1,
                    Label = "S = jusqu'à 3kg / 45cm",
                    MaxWeight = 3,
                    MaxPackageLength = 45,
                    TourPrice = 0.0,
                    Price = 3.0
               },
               new
               {
                    Name = "M",
                    Order = 2,
                    Label = "M = jusqu'à 10kg / 100cm",
                    MaxWeight = 10,
                    MaxPackageLength = 85,
                    TourPrice = 2.0,
                    Price = 5.0
               },
               new
               {
                    Name = "L",
                    Order = 3,
                    Label = "L = jusqu'à 20kg / 120cm",
                    MaxWeight = 20,
                    MaxPackageLength = 105,
                    TourPrice = 4.0,
                    Price = 7.0
               },
               new
               {
                    Name = "XL",
                    Order = 4,
                    Label = "XL = jusqu'à 30kg / 120cm",
                    MaxWeight = 30,
                    MaxPackageLength = 115,
                    TourPrice = 6.0,
                    Price = 9.0
               },
               new
               {
                    Name = "XXL",
                    Order = 5,
                    Label = "XXL = jusqu'à 40kg (sur devis)",
                    MaxWeight = 40,
                    MaxPackageLength = 500,
                    TourPrice = 8.0,
                    Price = 11.0
               },
               new
               {
                    Name = "XXXL",
                    Order = 6,
                    Label = "XXXL = jusqu'à 50kg (sur devis)",
                    MaxWeight = 50,
                    MaxPackageLength = 500,
                    TourPrice = 10.0,
                    Price = 13.0
               },
               new
               {
                    Name = "XXXXL",
                    Order = 7,
                    Label = "XXXXL = jusqu'à 60kg (sur devis)",
                    MaxWeight = 60,
                    MaxPackageLength = 500,
                    TourPrice = 12.0,
                    Price = 15.0
               }
          );
     }
}
