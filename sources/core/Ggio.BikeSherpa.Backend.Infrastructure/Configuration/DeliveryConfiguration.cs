using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
     public void Configure(EntityTypeBuilder<Delivery> builder)
     {
          builder.HasKey(d => d.Id);
          builder.Property(d => d.PricingStrategyEnum).HasConversion<int>().IsRequired();
          builder.Property(d => d.StatusEnum).HasConversion<int>().IsRequired();
          builder.Property(d => d.Code).IsRequired();
          builder.Property(d => d.CustomerId).IsRequired();
          builder.Property(d => d.Urgency).HasConversion<int>().IsRequired();
          builder.Property(d => d.TotalPrice).IsRequired();
          builder.Property(d => d.ReportId).IsRequired();
          builder.Property(d => d.Details).IsRequired();
          builder.Property(d => d.TotalWeight).IsRequired();
          builder.Property(d => d.HighestPackageLength).IsRequired();
          builder.Property(d => d.Size).IsRequired();
          builder.Property(d => d.ContractDate).IsRequired();
          builder.Property(d => d.StartDate).IsRequired();
          builder.ToTable("Deliveries");

          builder.OwnsMany(d => d.Steps, steps =>
          {
               steps.WithOwner().HasForeignKey("DeliveryId");
               steps.HasKey("Id");
               steps.Property(s => s.Id).ValueGeneratedNever();
               steps.Property(s => s.StepTypeEnum).HasConversion<int>().IsRequired();
               steps.Property(s => s.Order).IsRequired();
               steps.Property(s => s.StepAddress).IsRequired();
               steps.Property(s => s.StepZone).IsRequired();
               steps.Property(s => s.Distance).IsRequired();
               steps.Property(s => s.EstimatedDeliveryDate).IsRequired();
               steps.Property(s => s.RealDeliveryDate);
               steps.ToTable("DeliverySteps");
          });
     }
}
