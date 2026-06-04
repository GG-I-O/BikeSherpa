using Ggio.BikeSherpa.Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Configuration;

public class ParameterConfiguration : IEntityTypeConfiguration<Parameter>
{
     public void Configure(EntityTypeBuilder<Parameter> builder)
     {
         builder.HasKey(p => p.Key);
         builder.ToTable("Parameters");
         builder.Property(p => p.Key).HasMaxLength(100).IsRequired();
         builder.Property(p => p.Value).IsRequired();
         builder.HasData(
              new
              {
                   Key = ParameterRepository.VatRateKey,
                   Value = "20"
              }
         );
     }
}
