using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
     public void Configure(EntityTypeBuilder<Delivery> builder)
     {
          builder.HasKey(d => d.Id);
          builder.Property(d => d.Status).HasConversion<int>().IsRequired();
          builder.Property(d => d.Code).IsRequired();
          builder.Property(d => d.CustomerId).IsRequired();
          builder.Property(d => d.TotalPrice).IsRequired();
          builder.Property(d => d.ReportId).IsRequired();
          builder.Property(d => d.Packing).IsRequired();
          builder.Property(d => d.Details).IsRequired();
          builder.ToTable("Deliveries");
          
          builder.OwnsMany(d => d.Steps, steps =>
          {
               steps.WithOwner().HasForeignKey("DeliveryId");
               steps.HasKey("Id");
               steps.Property(s => s.Id).ValueGeneratedNever();
               steps.Property(s => s.StepType).HasConversion<int>().IsRequired();
               steps.Property(s => s.Order).IsRequired();
               steps.Property(s => s.DropoffAddress).IsRequired();
               steps.Property(s => s.DropoffZone).IsRequired();
               steps.Property(s => s.Distance).IsRequired();
               steps.Property(s => s.CourierId).IsRequired();
               steps.Property(s => s.Comment).IsRequired();
               steps.Property(s => s.FilePaths).IsRequired();
               steps.Property(s => s.EstimatedDeliveryDate).IsRequired();
               steps.Property(s => s.RealDeliveryDate).IsRequired();
               steps.ToTable("DeliverySteps");
          });
     }
}
